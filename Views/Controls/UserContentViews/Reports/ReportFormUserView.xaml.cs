namespace EcosenaApp.Views.Controls;

public partial class ReportFormUserView : ContentView
{
    public event EventHandler? GenerarReporteClicked;
    public event EventHandler? CancelarClicked;
    public event EventHandler? AbrirGaleriaClicked;
    public event EventHandler? AbrirCamaraClicked;

    public ReportFormUserView()
    {
        InitializeComponent();
        FechaLabel.Text = DateTime.Now.ToString("d 'de' MMMM 'de' yyyy, HH:mm",
            new System.Globalization.CultureInfo("es-CO"));
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
