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
    private readonly Dictionary<int, EstadoReporte> _ultimosEstados = new();
    private bool _primeraCarga = true;

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

            await NotificarCambiosDeEstadoAsync(reportes);
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

    private async Task NotificarCambiosDeEstadoAsync(List<ReportListResDto> reportes)
    {
        if (!_primeraCarga)
        {
            foreach (var reporte in reportes)
            {
                if (_ultimosEstados.TryGetValue(reporte.Id, out var estadoAnterior) && estadoAnterior != reporte.Estado)
                    await Toast.Make($"Tu reporte en {reporte.Ubicacion} cambió a {EstadoLegible(reporte.Estado)}").Show();
            }
        }

        _ultimosEstados.Clear();
        foreach (var reporte in reportes)
            _ultimosEstados[reporte.Id] = reporte.Estado;

        _primeraCarga = false;
    }

    private static string EstadoLegible(EstadoReporte estado) => estado switch
    {
        EstadoReporte.Pendiente => "Pendiente",
        EstadoReporte.EnProgreso => "En progreso",
        EstadoReporte.Resuelto => "Resuelto",
        _ => estado.ToString(),
    };

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
