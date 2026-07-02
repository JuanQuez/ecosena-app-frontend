using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Recovery;

namespace EcosenaApp.ViewModels.Auth;

public partial class ForgotPassViewModel : ObservableObject
{
    private readonly IRecoveryService _recoveryService;

    [ObservableProperty]
    private string documento = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string codigo = string.Empty;

    [ObservableProperty]
    private string nuevaContraseña = string.Empty;

    [ObservableProperty]
    private string confirmacionContraseña = string.Empty;

    [ObservableProperty]
    private bool enSegundoPaso;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyCanExecuteChangedFor(nameof(SolicitarCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReestablecerCommand))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public ForgotPassViewModel(IRecoveryService recoveryService)
    {
        _recoveryService = recoveryService;
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task SolicitarAsync()
    {
        if (string.IsNullOrWhiteSpace(Documento) || string.IsNullOrWhiteSpace(Email))
        {
            await Toast.Make("Completa documento y email.").Show();
            return;
        }

        IsBusy = true;
        try
        {
            var ok = await _recoveryService.SolicitarCodigoAsync(Documento, Email);
            if (ok)
            {
                EnSegundoPaso = true;
                await Toast.Make("Revisa tu correo para el código.").Show();
            }
            else
            {
                await Toast.Make("No se pudo enviar el código.").Show();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ReestablecerAsync()
    {
        if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(NuevaContraseña))
        {
            await Toast.Make("Completa el código y la nueva contraseña.").Show();
            return;
        }

        if (NuevaContraseña != ConfirmacionContraseña)
        {
            await Toast.Make("Las contraseñas no coinciden.").Show();
            return;
        }

        IsBusy = true;
        try
        {
            var ok = await _recoveryService.ReestablecerContraseñaAsync(Codigo, NuevaContraseña, ConfirmacionContraseña);
            await Toast.Make(ok ? "Contraseña actualizada." : "No se pudo reestablecer la contraseña.").Show();
            if (ok)
                await Shell.Current.GoToAsync("//LoginPage");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
