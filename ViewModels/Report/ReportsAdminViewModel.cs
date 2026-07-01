using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Report;
using EcosenaApp.Services.Report;
using System.Collections.ObjectModel;

namespace EcosenaApp.ViewModels.Report;

public partial class ReportsAdminViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    public ObservableCollection<ReportListResDto> Reportes { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private StatsReportDto? estadisticas;

    public ReportsAdminViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var reportes = await _reportService.GetAllReportsAsync();
            Reportes.Clear();
            foreach (var reporte in reportes)
                Reportes.Add(reporte);
        }
        catch (Exception)
        {
            await Toast.Make("No se pudieron cargar los reportes.").Show();
        }

        try
        {
            Estadisticas = await _reportService.GetEstadisticasAsync();
        }
        catch (Exception)
        {
            // Las estadísticas son secundarias a la lista de reportes: si fallan, no se
            // bloquea la vista con un segundo toast, los labels simplemente quedan en su último valor.
        }

        IsBusy = false;
    }
}
