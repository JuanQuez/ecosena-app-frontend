using EcosenaApp.Services.Auth;
using EcosenaApp.Models.Auth;

namespace EcosenaApp.Views.Auth;

public partial class SingUpPage : ContentPage
{
	private readonly IAuthService _authService;

	public SingUpPage()
	{
		InitializeComponent();
		_authService = IPlatformApplication.Current?.Services.GetService<IAuthService>()!;
	}

	private async void OnRegisterClicked(object sender, EventArgs e)
	{
		try
		{
			var documento = DocumentoEntry?.Text ?? string.Empty;
			var nombre = NombreEntry?.Text ?? string.Empty;
			var apellido = ApellidoEntry?.Text ?? string.Empty;
			var contraseña = ContraseñaEntry?.Text ?? string.Empty;
			var confirmacion = ConfirmacionContraseñaEntry?.Text ?? string.Empty;

			// Validar campos
			if (string.IsNullOrWhiteSpace(documento) || 
				string.IsNullOrWhiteSpace(nombre) || 
				string.IsNullOrWhiteSpace(apellido) || 
				string.IsNullOrWhiteSpace(contraseña) || 
				string.IsNullOrWhiteSpace(confirmacion))
			{
				await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
				return;
			}

			// Validar coincidencia de contraseñas
			if (contraseña != confirmacion)
			{
				await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
				return;
			}

			// Validar longitud de contraseña
			if (contraseña.Length < 6)
			{
				await DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres.", "OK");
				return;
			}

			var registerRequest = new RegisterReqDto
			{
				Documento = documento,
				Nombre = nombre,
				Apellido = apellido,
				Contraseña = contraseña,
				ConfirmacionContraseña = confirmacion
			};

			var success = await _authService.RegisterAsync(registerRequest);

			if (success)
			{
				await DisplayAlert("Éxito", "Registro completado. Inicia sesión con tus credenciales.", "OK");
				await Navigation.PopAsync();
			}
			else
			{
				await DisplayAlert("Error", "No pudimos registrarte. Verifica los datos e intenta nuevamente.", "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error al registrarse: {ex.Message}", "OK");
		}
	}

	private async void OnLoginGoBack(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}
