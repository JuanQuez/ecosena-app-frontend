namespace EcosenaApp.Views.Auth;

public partial class SingUpPage : ContentPage
{
	public SingUpPage()
	{
		InitializeComponent();
	}

    private async void OnLoginGoBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}