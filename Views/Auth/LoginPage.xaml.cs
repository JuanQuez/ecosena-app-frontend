using EcosenaApp.Views.Home;
using EcosenaApp.Services.Auth;

namespace EcosenaApp.Views.Auth;

public partial class LoginPage : ContentPage
{
	private readonly IAuthService _authService;

	public LoginPage()
	{
		InitializeComponent();
		_authService = IPlatformApplication.Current?.Services.GetService<IAuthService>()!;
	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		try
		{
			var documento = DocumentoEntry?.Text ?? string.Empty;
			var contraseña = ContraseñaEntry?.Text ?? string.Empty;

			if (string.IsNullOrWhiteSpace(documento) || string.IsNullOrWhiteSpace(contraseña))
			{
				await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
				return;
			}

			var result = await _authService.LoginAsync(documento, contraseña);

			if (result?.Jwt != null)
			{
				await Shell.Current.GoToAsync("//HostPage");
			}
			else
			{
				await DisplayAlert("Error", "Documento o contraseña incorrectos.", "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
		}
	}

	private async void OnSignUpTapped(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SingUpPage());
	}

	private async void OnForgotPassTapped(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ForgotPassPage());
	}   

	private async void OnGuestTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//HostPage");
	}
}
