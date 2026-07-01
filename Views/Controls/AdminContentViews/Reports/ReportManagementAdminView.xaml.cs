using EcosenaApp.Models.Report;

namespace EcosenaApp.Views.Controls;

public partial class ReportManagementAdminView : ContentView
{
    public event EventHandler? ActualizarEstadoClicked;
    public event EventHandler? PenalizarClicked;
    public event EventHandler? VolverClicked;

    public ReportManagementAdminView()
    {
        InitializeComponent();
    }

    public void MostrarReporte(ReportResDto reporte)
    {
        TituloLabel.Text = reporte.Titulo;
        DescripcionLabel.Text = reporte.Descripcion;
        EmisorLabel.Text = reporte.EmisorReporte;
        FotoImage.Source = string.IsNullOrEmpty(reporte.Foto) ? null : ImageSource.FromUri(new Uri(reporte.Foto));

        var (texto, colorKey) = reporte.Estado switch
        {
            EstadoReporte.Pendiente => ("Pendiente", "StatusPending"),
            EstadoReporte.EnProgreso => ("En progreso", "StatusUnderReview"),
            EstadoReporte.Resuelto => ("Resuelto", "StatusSolved"),
            _ => ("Pendiente", "StatusPending"),
        };

        EstadoLabel.Text = texto;
        EstadoBadge.BackgroundColor = (Color)Application.Current!.Resources[colorKey];
        AvanzarEstadoButton.IsEnabled = reporte.Estado != EstadoReporte.Resuelto;
    }

    private void OnActualizarEstadoTapped(object sender, EventArgs e)
    {
        ActualizarEstadoClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnPenalizarTapped(object sender, EventArgs e)
    {
        PenalizarClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnVolverTapped(object sender, EventArgs e)
    {
        VolverClicked?.Invoke(this, EventArgs.Empty);
    }
}
