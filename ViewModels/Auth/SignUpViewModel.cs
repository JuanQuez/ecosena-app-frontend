using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Auth;
using EcosenaApp.Services.Auth;

namespace EcosenaApp.ViewModels.Auth;

public partial class SignUpViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private bool isBusy;

    [ObservableProperty]
    private string documento = string.Empty;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string contraseña = string.Empty;

    [ObservableProperty]
    private string confirmacionContraseña = string.Empty;

    public bool IsNotBusy => !IsBusy;

    public SignUpViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Documento) ||
            string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(Apellido) ||
            string.IsNullOrWhiteSpace(Contraseña) ||
            string.IsNullOrWhiteSpace(ConfirmacionContraseña))
        {
            await Toast.Make("Completa todos los campos.").Show();
            return;
        }

        if (Contraseña != ConfirmacionContraseña)
        {
            await Toast.Make("Las contraseñas no coinciden.").Show();
            return;
        }

        if (Contraseña.Length < 6)
        {
            await Toast.Make("Mínimo 6 caracteres.").Show();
            return;
        }

        IsBusy = true;
        try
        {
            var request = new RegisterReqDto
            {
                Documento = Documento,
                Nombre = Nombre,
                Apellido = Apellido,
                Contraseña = Contraseña,
                ConfirmacionContraseña = ConfirmacionContraseña
            };

            var success = await _authService.RegisterAsync(request);
            if (success)
            {
                await Toast.Make("Registro exitoso. Inicia sesión.").Show();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await Toast.Make("No pudimos registrarte. Verifica tus datos.").Show();
            }
        }
        catch (Exception)
        {
            await Toast.Make("Sin conexión. Verifica tu internet.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
