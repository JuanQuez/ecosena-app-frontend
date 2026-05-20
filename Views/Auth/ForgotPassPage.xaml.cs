namespace EcosenaApp.Views.Auth;

public partial class ForgotPassPage : ContentPage
{
	public ForgotPassPage()
	{
		InitializeComponent();
	}

    private async void OnLoginGoBcak(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}