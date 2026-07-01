using CommunityToolkit.Maui.Alerts;
using EcosenaApp.Services.Session;

namespace EcosenaApp.Views.Controls;

public partial class TopBarView : ContentView
{
    public TopBarView()
    {
        InitializeComponent();
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        var userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        if (userSession != null && !userSession.IsAuthenticated)
        {
            await Toast.Make("Ingresa a tu cuenta.").Show();
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Se envuelve en un NavigationPage propio para aislar el push/pop de ProfilePage/EditProfilePage
        // de la pila de Shell: un bug no resuelto de MAUI Shell (dotnet/maui#21570) provoca
        // "Ambiguous routes matched" al empujar 2+ páginas directamente sobre Shell y luego hacer Pop.
        var profilePage = new Views.Profile.ProfilePage();
        NavigationPage.SetHasNavigationBar(profilePage, false);
        await Navigation.PushAsync(new NavigationPage(profilePage));
    }
}
