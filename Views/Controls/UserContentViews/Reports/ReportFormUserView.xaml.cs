namespace EcosenaApp.Views.Controls;

public partial class ReportFormUserView : ContentView
{
    public event EventHandler? GenerarReporteClicked;
    public event EventHandler? CancelarClicked;
    public event EventHandler? AbrirGaleriaClicked;
    public event EventHandler? AbrirCamaraClicked;

    public string Titulo => TituloEntry.Text ?? string.Empty;
    public string Descripcion => DescripcionEditor.Text ?? string.Empty;
    public int AmbienteIndex => AmbientePicker.SelectedIndex;

    public ReportFormUserView()
    {
        InitializeComponent();
        FechaLabel.Text = DateTime.Now.ToString("d 'de' MMMM 'de' yyyy, HH:mm",
            new System.Globalization.CultureInfo("es-CO"));
    }

    public void SetAmbientes(IEnumerable<string> nombres)
    {
        AmbientePicker.ItemsSource = nombres.ToList();
    }

    public void SetFotoPreview(ImageSource source)
    {
        FotoPreviewImage.Behaviors.Clear();
        FotoPreviewImage.Source = source;
    }

    public void SetBloqueado(bool bloqueado)
    {
        SancionLabel.IsVisible = bloqueado;
        TituloEntry.IsEnabled = !bloqueado;
        DescripcionEditor.IsEnabled = !bloqueado;
        AmbientePicker.IsEnabled = !bloqueado;
        GenerarReporteButton.IsEnabled = !bloqueado;
    }

    public void LimpiarFormulario()
    {
        TituloEntry.Text = string.Empty;
        DescripcionEditor.Text = string.Empty;
        AmbientePicker.SelectedIndex = -1;
        FotoPreviewImage.Source = "icon_gallery_report.svg";
    }

    private void OnGenerarReporteTapped(object sender, EventArgs e)
    {
        GenerarReporteClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelarTapped(object sender, EventArgs e)
    {
        CancelarClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnAbrirGaleriaTapped(object sender, TappedEventArgs e)
    {
        AbrirGaleriaClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnAbrirCamaraTapped(object sender, TappedEventArgs e)
    {
        AbrirCamaraClicked?.Invoke(this, EventArgs.Empty);
    }
}
