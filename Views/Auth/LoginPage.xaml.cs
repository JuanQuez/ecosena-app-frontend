using EcosenaApp.Views.Home;

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

    private async void OnForgotPassTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPassPage());
    }   
    private async void OnHomeDefaultTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }   
    
    private async void OnGuestTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}