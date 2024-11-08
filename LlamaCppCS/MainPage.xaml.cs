using LlamaCppCS.Core;

namespace LlamaCppChat {
	public partial class MainPage : ContentPage {
		private ChatService chatService = new ChatService();

		public MainPage() {
			InitializeComponent();
		}

		async private void SubmitComplete_Clicked(object sender, EventArgs e) {
			try {
				UserRequest.IsEnabled = false;
				SubmitComplete.IsEnabled = false;
				SubmitStreaming.IsEnabled = false;
				AssistantResponse.Text = await chatService.SendMessageAsync(UserRequest.Text);
			} catch (Exception exc) {
				System.Diagnostics.Debug.WriteLine(exc);
			} finally {
				UserRequest.IsEnabled = true;
				SubmitComplete.IsEnabled = true;
				SubmitStreaming.IsEnabled = true;
			}
		}

		async private void SubmitStreaming_Clicked(object sender, EventArgs e) {
			try {
				UserRequest.IsEnabled = false;
				SubmitComplete.IsEnabled = false;
				SubmitStreaming.IsEnabled = false;
				await foreach (string message in chatService.SendMessageStreamAsync(UserRequest.Text)) {
					AssistantResponse.Text = message;
				}
			} catch (Exception exc) {
				System.Diagnostics.Debug.WriteLine(exc);
			} finally {
				UserRequest.IsEnabled = true;
				SubmitComplete.IsEnabled = true;
				SubmitStreaming.IsEnabled = true;
			}
		}
	}
}
