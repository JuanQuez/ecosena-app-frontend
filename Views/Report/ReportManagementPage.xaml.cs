using EcosenaApp.ViewModels.Report;

namespace EcosenaApp.Views.Report;

public partial class ReportManagementPage : ContentPage
{
    private readonly ReportManagementViewModel? _viewModel;

    public ReportManagementPage(int id)
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<ReportManagementViewModel>();
        BindingContext = _viewModel;
        if (_viewModel != null)
            _viewModel.ReporteId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel == null)
            return;

        await _viewModel.LoadCommand.ExecuteAsync(null);
        if (_viewModel.Reporte != null)
            ManagementView.MostrarReporte(_viewModel.Reporte);
    }

    private async void OnActualizarEstadoTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        await _viewModel.AvanzarEstadoCommand.ExecuteAsync(null);
        if (_viewModel.Reporte != null)
            ManagementView.MostrarReporte(_viewModel.Reporte);
    }

    private async void OnPenalizarTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        bool confirm = await DisplayAlert("Confirmar", "¿Penalizar al emisor de este reporte?", "Sí", "No");
        if (!confirm)
            return;

        await _viewModel.PenalizarCommand.ExecuteAsync(null);
    }

    private async void OnVolverTapped(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
