using EcosenaApp.ViewModels.Profile;

namespace EcosenaApp.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetService<ProfileViewModel>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
            vm.LoadProfileCommand.Execute(null);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");
        if (confirm && BindingContext is ProfileViewModel vm)
            vm.LogoutCommand.Execute(null);
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage());
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
}
