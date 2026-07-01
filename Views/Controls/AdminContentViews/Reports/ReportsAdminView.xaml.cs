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
            _viewModel.LoadCommand.Execute(null);
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
