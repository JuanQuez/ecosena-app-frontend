using EcosenaApp.Models.Report;
using EcosenaApp.ViewModels.Report;

namespace EcosenaApp.Views.Controls;

public partial class ReportsAdminView : ContentView
{
    private readonly ReportsAdminViewModel? _viewModel;

    public ReportsAdminView()
    {
        InitializeComponent();

        _viewModel = IPlatformApplication.Current?.Services.GetService<ReportsAdminViewModel>();
        BindingContext = _viewModel;
        ReportesCollection.ItemsSource = _viewModel?.Reportes;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.LoadCommand.Execute(null);
        }
    }

    public void Refresh() => _viewModel?.LoadCommand.Execute(null);

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ReportsAdminViewModel.Estadisticas) || _viewModel?.Estadisticas is not StatsReportDto stats)
            return;

        StatTotalLabel.Text = stats.ReportesHechosMes.ToString();
        StatPendingLabel.Text = stats.ReportesPendientes.ToString();
        StatInProgressLabel.Text = stats.ReportesEnProgreso.ToString();
        StatSolvedLabel.Text = stats.ReportesResueltosMes.ToString();
    }

    private async void OnReporteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ReportListResDto reporte)
        {
            ReportesCollection.SelectedItem = null;
            await Navigation.PushAsync(new Views.Report.ReportManagementPage(reporte.Id));
        }
    }
}
