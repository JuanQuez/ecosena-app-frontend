namespace EcosenaApp.Views.Auth;
using EcosenaApp.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
        InitializeComponent();
	}

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SingUpPage());
    }
}