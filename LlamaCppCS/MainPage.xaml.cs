using LlamaCppCS.Core;

namespace LlamaCppChat {
	public partial class MainPage : ContentPage {
		private ChatService chatService = new ChatService();

		public MainPage() {
			InitializeComponent();
			LoadModel.IsEnabled = false;
			UnloadModel.IsEnabled = false;
			SubmitComplete.IsEnabled = false;
			SubmitStreaming.IsEnabled = false;
		}

		private async void SelectModel_Clicked(object sender, EventArgs e) {
			var result = await FilePicker.PickAsync(new PickOptions {
				PickerTitle = "Seleziona il file del modello"
			});

			if (result != null) {
				ModelPath.Text = result.FullPath.Trim();
			}

			LoadModel.IsEnabled = true;
			UnloadModel.IsEnabled = false;
			SubmitComplete.IsEnabled = false;
			SubmitStreaming.IsEnabled = false;
		}

		private async void LoadModel_Clicked(object sender, EventArgs e) {
			if (String.IsNullOrWhiteSpace(ModelPath.Text)) {
				// Mostra un messaggio di informativo all'utente che indica di selezionare il modello da caricare.
				await DisplayAlert("Errore", "Seleziona un modello da caricare.", "OK");
				return;
			} else if (!File.Exists(ModelPath.Text.Trim())) {
				// Mostra un messaggio di errore all'utente che indica che il file del modello non esiste.
				await DisplayAlert("Errore", "Il file del modello non esiste.", "OK");
				return;
			} else if (ModelPath.Text.Trim() != chatService.ModelPath) {
				LoadModel.IsEnabled = !(await chatService.LoadModel(ModelPath.Text));
				UnloadModel.IsEnabled = !LoadModel.IsEnabled;
				SubmitComplete.IsEnabled = !LoadModel.IsEnabled;
				SubmitStreaming.IsEnabled = !LoadModel.IsEnabled;
			}
		}

		private async void UnloadModel_Clicked(object sender, EventArgs e) {
			LoadModel.IsEnabled = !(await chatService.UnloadModel());
			UnloadModel.IsEnabled = !LoadModel.IsEnabled;
			SubmitComplete.IsEnabled = !LoadModel.IsEnabled;
			SubmitStreaming.IsEnabled = !LoadModel.IsEnabled;
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
