using EcosenaApp.ViewModels.Auth;

namespace EcosenaApp.Views.Auth;

public partial class SingUpPage : ContentPage
{
	public SingUpPage()
	{
		InitializeComponent();
		BindingContext = IPlatformApplication.Current?.Services.GetService<SignUpViewModel>();
	}

	private async void OnLoginGoBack(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}
