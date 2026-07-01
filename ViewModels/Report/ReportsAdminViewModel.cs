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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotExporting))]
    [NotifyCanExecuteChangedFor(nameof(ExportarExcelCommand))]
    private bool isExporting;

    public bool IsNotExporting => !IsExporting;

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
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNotExporting))]
    private async Task ExportarExcelAsync()
    {
        IsExporting = true;
        try
        {
            var resultado = await _reportService.ExportarExcelAsync();
            if (resultado is null)
            {
                await Toast.Make("No se pudo exportar el Excel.").Show();
                return;
            }

            var (bytes, fileName) = resultado.Value;
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, bytes);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Reportes",
                File = new ShareFile(path)
            });
        }
        catch (Exception)
        {
            await Toast.Make("No se pudo exportar el Excel.").Show();
        }
        finally
        {
            IsExporting = false;
        }
    }
}
