using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Auth;
using EcosenaApp.Services.Profile;
using EcosenaApp.Services.Session;

namespace EcosenaApp.ViewModels.Profile;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly IUserSession _userSession;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string? programa;

    [ObservableProperty]
    private int? ficha;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string fechaNacimiento = string.Empty;

    [ObservableProperty]
    private ImageSource fotoPerfil = ImageSource.FromFile("icon_profile.svg");

    public ProfileViewModel(IAuthService authService, IProfileService profileService, IUserSession userSession)
    {
        _authService = authService;
        _profileService = profileService;
        _userSession = userSession;
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        IsBusy = true;
        try
        {
            var profile = await _profileService.GetProfileAsync();
            if (profile == null)
            {
                await Toast.Make("Sesión expirada.").Show();
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            Nombre = profile.Nombre;
            Programa = profile.Programa;
            Ficha = profile.Ficha;
            Email = profile.Email;
            FechaNacimiento = profile.FechaNacimiento;

            if (!string.IsNullOrEmpty(profile.FotoPerfil))
                FotoPerfil = ImageSource.FromUri(new Uri(profile.FotoPerfil));
        }
        catch (Exception)
        {
            await Toast.Make("No se pudo cargar el perfil.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        _userSession.Clear();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
