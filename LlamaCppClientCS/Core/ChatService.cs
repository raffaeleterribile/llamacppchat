using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace LlamaCppClientCS.Core {
	internal class ChatService {
		private const string SERVER_URL = "http://localhost:8080";
		private const string API_KEY = "no-api-key";
		private const string MODEL_ID = "phi3:mini";
		private const string SYSTEM_MESSAGE = "Sei un utile assistente";

		private List<ChatMessage> history = new List<ChatMessage>();
		private ApiKeyCredential credentials = new(API_KEY);

		private OpenAIClientOptions options = new OpenAIClientOptions() {
			Endpoint = new Uri(SERVER_URL)
		};

		internal async Task<string> SendMessageAsync(string message) {
			SystemChatMessage system = new SystemChatMessage(SYSTEM_MESSAGE);

			List<ChatMessage> messages = new List<ChatMessage>() { system };

			history.Add(new UserChatMessage(message));

			messages.AddRange(history);

			OpenAIClient clientConfigurator = new OpenAIClient(credentials, options);

			ChatClient client = clientConfigurator.GetChatClient(MODEL_ID);

			ChatCompletion completion = await client.CompleteChatAsync(messages.ToArray());

			string response = completion.Content[0].Text;

			history.Add(new AssistantChatMessage(response));

			return response;
		}
		internal async IAsyncEnumerable<string> SendMessageStreamAsync(string message) {
			SystemChatMessage system = new SystemChatMessage(SYSTEM_MESSAGE);

			List<ChatMessage> messages = new List<ChatMessage>() { system };

			history.Add(new UserChatMessage(message));

			messages.AddRange(history);

			OpenAIClient clientConfigurator = new OpenAIClient(credentials, options);

			ChatClient client = clientConfigurator.GetChatClient(MODEL_ID);

			AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(messages);

			string partialResponse = string.Empty;
			await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates) {
				if (completionUpdate.ContentUpdate.Count > 0) {
					partialResponse += completionUpdate.ContentUpdate[0].Text;
					yield return partialResponse;
				}
			}

			history.Add(new AssistantChatMessage(partialResponse));
		}
	}
}
