using LLama;
using LLama.Common;
using LLama.Native;

namespace LlamaCppCS.Core {
	internal class ChatService {
		private const string MODEL_PATH = "../Phi-3-mini-4k-instruct-q4.gguf";

		private const string SYSTEM_MESSAGE = "Ciao! Come posso aiutarti?";

		private ChatHistory chatHistory = new ChatHistory();
		private InteractiveExecutor executor;

		private InferenceParams inferenceParams = new InferenceParams() {
			MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
			AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
		};

		internal ChatService() {
			// Messaggio di log inserito solo per verificare il backend utilizzato
			NativeLibraryConfig.All.WithLogCallback(delegate (LLamaLogLevel level, string message) { System.Diagnostics.Debug.WriteLine($"{level}: {message}"); });
			NativeLibraryConfig.All.ForEach(config => System.Diagnostics.Debug.WriteLine($"{config.ToString()}"));

			var parameters = new ModelParams(MODEL_PATH) {
				ContextSize = 1024, // The longest length of chat as memory.
				GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
			};
			using var model = LLamaWeights.LoadFromFile(parameters);
			using var context = model.CreateContext(parameters);
			executor = new InteractiveExecutor(context);

			// Add chat histories as prompt to tell AI how to act.
			chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
		}

		internal async Task<string> SendMessageAsync(string message) {
			ChatSession session = new(executor, chatHistory);

			chatHistory.AddMessage(AuthorRole.User, message);

			ChatHistory messages = new ChatHistory();
			messages.AddMessage(AuthorRole.System, SYSTEM_MESSAGE);
			messages.Messages.AddRange(chatHistory.Messages);

			// Generate the response streamingly and aggregate the generated texts.
			string response = "";
			await foreach (var text in session.ChatAsync(messages, inferenceParams)) {
				System.Diagnostics.Debug.WriteLine(text);
				response += text;
			}

			chatHistory.AddMessage(AuthorRole.Assistant, response);

			return response;
		}

		internal async IAsyncEnumerable<string> SendMessageStreamAsync(string message) {
			ChatSession session = new(executor, chatHistory);

			chatHistory.AddMessage(AuthorRole.User, message);

			ChatHistory messages = new ChatHistory();
			messages.AddMessage(AuthorRole.System, SYSTEM_MESSAGE);
			messages.Messages.AddRange(chatHistory.Messages);

			// Generate the response streamingly.
			string response = "";
			await foreach (var text in session.ChatAsync(messages, inferenceParams)) {
				System.Diagnostics.Debug.WriteLine(text);
				response += text;
				yield return response;
			}

			chatHistory.AddMessage(AuthorRole.Assistant, response);
		}
	}
}

//this.config = config;
//this.serverUrl = this.config["ServerUrl"];
//this.client = new OllamaApiClient(this.serverUrl);
//this.chatHistory = new List<Message>() {
//								new Message {
//									Role = ChatRole.System,
//									Content = "Ciao! Come posso aiutarti?"
//								}
//							};

//internal async Task<string> SendMessageAsync(string message) {

//	chatHistory.Add(new Message {
//		Role = ChatRole.User,
//		Content = message
//	});

//	var request = new ChatRequest {
//		Model = MODEL_ID,
//		Stream = false,
//		Messages = chatHistory
//	};

//	var response = await client.Chat(request).StreamToEnd();

//	chatHistory.Add(new Message {
//		Role = ChatRole.Assistant,
//		Content = message
//	});

//	return response.Message.Content;
//}

//internal async IAsyncEnumerable<string> SendMessageStreamAsync(string message) {

//	chatHistory.Add(new Message {
//		Role = ChatRole.User,
//		Content = message
//	});

//	var request = new ChatRequest {
//		Model = MODEL_ID,
//		Stream = true,
//		Messages = chatHistory
//	};

//	await foreach (var response in client.Chat(request)) {
//		yield return response.Message.Content;
//	}

//	chatHistory.Add(new Message {
//		Role = ChatRole.Assistant,
//		Content = message
//	});
//}

