using EcosenaApp.Services.Session;
using EcosenaApp.ViewModels.Auth;

namespace EcosenaApp.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = IPlatformApplication.Current?.Services.GetService<LoginViewModel>();
	}

	private async void OnSignUpTapped(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SingUpPage());
	}

	private async void OnForgotPassTapped(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ForgotPassPage());
	}

	private async void OnGuestTapped(object sender, EventArgs e)
	{
		var session = IPlatformApplication.Current?.Services.GetService<IUserSession>();
		session?.SetGuest();
		await Shell.Current.GoToAsync("//HostPage");
	}
}
