namespace EcosenaApp.Views.Controls;

public partial class EditBlogEntryAdminView : ContentView
{
    public event EventHandler? ActualizarEntradaClicked;
    public event EventHandler? CancelarClicked;
    public event EventHandler? CambiarPortadaClicked;

    public string Titulo => TituloEntry.Text ?? string.Empty;
    public string Contenido => ContenidoEditor.Text ?? string.Empty;

    public EditBlogEntryAdminView()
    {
        InitializeComponent();
    }

    public void CargarEntrada(string titulo, string contenido, ImageSource? portada = null)
    {
        TituloEntry.Text = titulo;
        ContenidoEditor.Text = contenido;

        if (portada is not null)
        {
            PortadaImage.Behaviors.Clear();
            PortadaImage.Source = portada;
            PortadaImage.Aspect = Aspect.AspectFill;
        }
    }

    public void SetPortada(ImageSource source)
    {
        PortadaImage.Behaviors.Clear();
        PortadaImage.Source = source;
        PortadaImage.Aspect = Aspect.AspectFill;
    }

    private void OnCambiarPortadaTapped(object sender, TappedEventArgs e)
    {
        CambiarPortadaClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnActualizarTapped(object sender, EventArgs e)
    {
        ActualizarEntradaClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelarTapped(object sender, EventArgs e)
    {
        CancelarClicked?.Invoke(this, EventArgs.Empty);
    }
}
