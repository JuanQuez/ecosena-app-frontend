namespace EcosenaApp.Views.Controls;

public partial class CreateBlogEntryAdminView : ContentView
{
    public event EventHandler? PublicarEntradaClicked;
    public event EventHandler? GuardarBorradorClicked;
    public event EventHandler? CancelarClicked;
    public event EventHandler? AbrirPortadaClicked;

    public string Titulo => TituloEntry.Text ?? string.Empty;
    public string Contenido => ContenidoEditor.Text ?? string.Empty;

    public CreateBlogEntryAdminView()
    {
        InitializeComponent();
    }

    public void SetPortada(ImageSource source)
    {
        PortadaImage.Behaviors.Clear();
        PortadaImage.Source = source;
        PortadaImage.Aspect = Aspect.AspectFill;
    }

    public void LimpiarFormulario()
    {
        TituloEntry.Text = string.Empty;
        ContenidoEditor.Text = string.Empty;
        PortadaImage.Source = "icon_gallery_report.svg";
        PortadaImage.Behaviors.Add(new CommunityToolkit.Maui.Behaviors.IconTintColorBehavior
        {
            TintColor = (Color)Application.Current!.Resources["GrayMidnight"]
        });
    }

    private void OnAbrirPortadaTapped(object sender, TappedEventArgs e)
    {
        AbrirPortadaClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnPublicarTapped(object sender, EventArgs e)
    {
        PublicarEntradaClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnGuardarBorradorTapped(object sender, EventArgs e)
    {
        GuardarBorradorClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelarTapped(object sender, EventArgs e)
    {
        CancelarClicked?.Invoke(this, EventArgs.Empty);
    }
}
