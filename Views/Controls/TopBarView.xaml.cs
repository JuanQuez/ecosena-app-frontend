namespace EcosenaApp.Views.Controls;

public partial class TopBarView : ContentView
{
    public TopBarView()
    {
        InitializeComponent();
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.Profile.ProfilePage());
    }
}
