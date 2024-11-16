using LLama;
using LLama.Common;
using LLama.Native;

namespace LlamaCppCS.Core {
	internal class ChatService {
		private const string SYSTEM_MESSAGE = "Ciao! Come posso aiutarti?";

		private bool isModelLoaded = false;
		private string? modelPath;

		private ChatHistory chatHistory = new ChatHistory();
		private InteractiveExecutor? executor;

		private InferenceParams inferenceParams = new InferenceParams() {
			MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
			AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
		};

		internal bool IsModelLoaded {
			get {
				return isModelLoaded;
			}
		}

		internal string? ModelPath {
			get {
				return modelPath;
			}
		}

		static ChatService() {
			// Messaggio di log inserito solo per verificare il backend utilizzato
			NativeLibraryConfig.All.WithLogCallback(delegate (LLamaLogLevel level, string message) {
				System.Diagnostics.Debug.WriteLine($"{level}: {message}");
			});
			NativeLibraryConfig.All.ForEach(config => System.Diagnostics.Debug.WriteLine($"{config.ToString()}"));

			// Il mio attuale computer non supporta le istruzioni AVX, CUDA e Vulkan
			NativeLibraryConfig.All.WithAvx(AvxLevel.None);
			NativeLibraryConfig.All.WithCuda(false);
			NativeLibraryConfig.All.WithVulkan(false);

			// È necessario specificare il percorso delle librerie native di LlamaCpp utilizzate perché
			// il pacchetto NuGet "LLamaSharp.Backend.Cpu" cerca di copiare le librerie native tutte nella stessa cartella, 
			// causando l'errore "NETSDK1152: Found multiple publish output files with the same relative path"
			NativeLibraryConfig.All.WithLibrary(".\\LlamaCppCPU\\llama.dll", ".\\LlamaCppCPU\\llava.dll");
		}

		internal async Task<bool> LoadModel(string modelPath) {
			if (String.IsNullOrWhiteSpace(modelPath)) {
				return isModelLoaded;
			} else if (!File.Exists(modelPath.Trim())) {
				return isModelLoaded;
			} else if (modelPath.Trim() != this.modelPath) {
				await UnloadModel();
			}

			modelPath = modelPath.Trim();

			var parameters = new ModelParams(modelPath) {
				ContextSize = 1024, // The longest length of chat as memory.
				GpuLayerCount = 0 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
			};
			using var model = LLamaWeights.LoadFromFile(parameters);
			using var context = model.CreateContext(parameters);
			executor = new InteractiveExecutor(context);

			// Add chat histories as prompt to tell AI how to act.
			chatHistory.Messages.Clear();

			isModelLoaded = true;
			this.modelPath = modelPath;
			return isModelLoaded;
		}

		internal async Task<bool> UnloadModel() {
			executor?.Context?.Dispose();

			executor = null;
			chatHistory.Messages.Clear();

			modelPath = null;
			isModelLoaded = false;
			return await Task.FromResult(isModelLoaded);
		}

		internal async Task<string> SendMessageTransactionalAsync(string message) {
			if (executor == null) {
				return await Task.FromResult("");
			}

			ChatHistory messages = new ChatHistory();
			messages.AddMessage(AuthorRole.System, SYSTEM_MESSAGE);
			messages.Messages.AddRange(chatHistory.Messages);

			ChatSession session = new(executor, messages);

			var chatMessage = new ChatHistory.Message(AuthorRole.User, message);
			chatHistory.AddMessage(AuthorRole.User, message);

			// Generate the response streamingly and aggregate the generated texts.
			string response = "";
			await foreach (var text in session.ChatAsync(chatMessage, inferenceParams)) {
				System.Diagnostics.Debug.WriteLine(text);
				response += text;
			}

			chatHistory.AddMessage(AuthorRole.Assistant, response);

			return response;
		}

		internal async IAsyncEnumerable<string> SendMessageStreamingAsync(string message) {
			if (executor == null) {
				yield break;
			}

			ChatHistory messages = new ChatHistory();
			messages.AddMessage(AuthorRole.System, SYSTEM_MESSAGE);
			messages.Messages.AddRange(chatHistory.Messages);

			ChatSession session = new(executor, messages);

			var chatMessage = new ChatHistory.Message(AuthorRole.User, message);
			chatHistory.AddMessage(AuthorRole.User, message);

			// Generate the response streamingly.
			string response = "";
			await foreach (var text in session.ChatAsync(chatMessage, inferenceParams)) {
				System.Diagnostics.Debug.WriteLine(text);
				response += text;
				yield return response;
			}

			chatHistory.AddMessage(AuthorRole.Assistant, response);
		}
	}
}
