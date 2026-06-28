namespace EcosenaApp.Views.Controls;

public enum EstadoReporte { EnRevision, EnProgreso, Pendiente }

public partial class ReportManagementAdminView : ContentView
{
    public event EventHandler<EstadoReporte>? ActualizarEstadoClicked;
    public event EventHandler? VolverClicked;

    private EstadoReporte _estadoSeleccionado = EstadoReporte.EnProgreso;

    public ReportManagementAdminView()
    {
        InitializeComponent();
        ActualizarIndicadores();
    }

    private void OnEnRevisionTapped(object sender, TappedEventArgs e)
    {
        _estadoSeleccionado = EstadoReporte.EnRevision;
        ActualizarIndicadores();
    }

    private void OnEnProgresoTapped(object sender, TappedEventArgs e)
    {
        _estadoSeleccionado = EstadoReporte.EnProgreso;
        ActualizarIndicadores();
    }

    private void OnPendienteTapped(object sender, TappedEventArgs e)
    {
        _estadoSeleccionado = EstadoReporte.Pendiente;
        ActualizarIndicadores();
    }

    private void ActualizarIndicadores()
    {
        var solved = (Color)Application.Current!.Resources["StatusSolved"];
        var review = (Color)Application.Current!.Resources["StatusUnderReview"];
        var pending = (Color)Application.Current!.Resources["StatusPending"];

        SetIndicador(IndicadorEnRevision, DotEnRevision, solved,
            _estadoSeleccionado == EstadoReporte.EnRevision);
        SetIndicador(IndicadorEnProgreso, DotEnProgreso, review,
            _estadoSeleccionado == EstadoReporte.EnProgreso);
        SetIndicador(IndicadorPendiente, DotPendiente, pending,
            _estadoSeleccionado == EstadoReporte.Pendiente);
    }

    private static void SetIndicador(Border indicador, BoxView dot, Color color, bool seleccionado)
    {
        indicador.Stroke = new SolidColorBrush(color);
        indicador.BackgroundColor = seleccionado ? color : Colors.Transparent;
        dot.BackgroundColor = seleccionado ? Colors.White : Colors.Transparent;
    }

    private void OnActualizarEstadoTapped(object sender, EventArgs e)
    {
        ActualizarEstadoClicked?.Invoke(this, _estadoSeleccionado);
    }

    private void OnVolverTapped(object sender, EventArgs e)
    {
        VolverClicked?.Invoke(this, EventArgs.Empty);
    }
}
