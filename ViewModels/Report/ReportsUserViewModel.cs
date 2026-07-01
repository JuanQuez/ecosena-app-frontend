using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Report;
using EcosenaApp.Services.Report;
using System.Collections.ObjectModel;

namespace EcosenaApp.ViewModels.Report;

public partial class ReportsUserViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    public ObservableCollection<ReportListResDto> MisReportes { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool mostrarFormulario;

    public ReportsUserViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var reportes = await _reportService.GetMyReportsAsync();
            MisReportes.Clear();
            foreach (var reporte in reportes)
                MisReportes.Add(reporte);
        }
        catch (Exception)
        {
            await Toast.Make("No se pudieron cargar tus reportes.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Nombre distinto a la propiedad MostrarFormulario: [ObservableProperty] ya genera
    // un miembro público "MostrarFormulario", y un método con el mismo nombre colisiona (CS0102).
    [RelayCommand]
    private void AbrirFormulario()
    {
        MostrarFormulario = true;
    }

    [RelayCommand]
    private void VolverALista()
    {
        MostrarFormulario = false;
    }
}
