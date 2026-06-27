using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Auth;

namespace EcosenaApp.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool isBusy;

    [ObservableProperty]
    private string documento = string.Empty;

    [ObservableProperty]
    private string contraseña = string.Empty;

    public bool IsNotBusy => !IsBusy;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Documento) || string.IsNullOrWhiteSpace(Contraseña))
        {
            await Toast.Make("Completa todos los campos.").Show();
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync(Documento, Contraseña);
            if (result?.Jwt != null)
                await Shell.Current.GoToAsync("//HostPage");
            else
                await Toast.Make("Documento o contraseña incorrectos.").Show();
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
