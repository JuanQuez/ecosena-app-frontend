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

    [ObservableProperty]
    private bool rememberMe;

    public bool IsNotBusy => !IsBusy;

    private const string PrefRememberMe = "remember_me";
    private const string PrefDocumento = "saved_documento";
    private const string KeyPassword = "saved_password";

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        _ = LoadSavedCredentialsAsync();
    }

    private async Task LoadSavedCredentialsAsync()
    {
        RememberMe = Preferences.Get(PrefRememberMe, false);
        if (!RememberMe) return;

        Documento = Preferences.Get(PrefDocumento, string.Empty);
        Contraseña = await SecureStorage.GetAsync(KeyPassword) ?? string.Empty;
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
            {
                if (RememberMe)
                {
                    Preferences.Set(PrefRememberMe, true);
                    Preferences.Set(PrefDocumento, Documento);
                    await SecureStorage.SetAsync(KeyPassword, Contraseña);
                }
                else
                {
                    Preferences.Remove(PrefRememberMe);
                    Preferences.Remove(PrefDocumento);
                    SecureStorage.Remove(KeyPassword);
                }
                await Shell.Current.GoToAsync("//HostPage");
            }
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
