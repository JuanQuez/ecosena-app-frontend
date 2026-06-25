using EcosenaApp.Services.Auth;

namespace EcosenaApp.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthService _authService;

    public ProfilePage()
    {
        InitializeComponent();
        _authService = IPlatformApplication.Current?.Services.GetService<IAuthService>()!;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Ir a editar perfil", "OK");
    }

    private async void OnNotificationsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Ir a notificaciones", "OK");
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Ir a privacidad", "OK");
    }

    private async void OnReportProblemTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Ir a reportar problema", "OK");
    }

    private async void OnTermsConditionsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Ir a términos y condiciones", "OK");
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
