namespace EcosenaApp.Views.Profile;

public partial class InfoTextPage : ContentPage
{
    public InfoTextPage(string titulo, string contenido)
    {
        InitializeComponent();
        TitleLabel.Text = titulo;
        BodyLabel.Text = contenido;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
