using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Report;
using EcosenaApp.Services.Report;

namespace EcosenaApp.ViewModels.Report;

public partial class ReportManagementViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    public int ReporteId { get; set; }

    [ObservableProperty]
    private ReportResDto? reporte;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstaProcesando))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotAvanzando))]
    [NotifyPropertyChangedFor(nameof(EstaProcesando))]
    private bool isAvanzando;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotPenalizando))]
    [NotifyPropertyChangedFor(nameof(EstaProcesando))]
    private bool isPenalizando;

    public bool IsNotAvanzando => !IsAvanzando;
    public bool IsNotPenalizando => !IsPenalizando;
    public bool EstaProcesando => IsBusy || IsAvanzando || IsPenalizando;

    public bool Penalizado { get; private set; }

    public ReportManagementViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Reporte = await _reportService.GetReportAsync(ReporteId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AvanzarEstadoAsync()
    {
        IsAvanzando = true;
        try
        {
            var ok = await _reportService.UpdateEstadoAsync(ReporteId);
            if (ok)
                Reporte = await _reportService.GetReportAsync(ReporteId);
            else
                await Toast.Make("No se pudo actualizar el estado.").Show();
        }
        finally
        {
            IsAvanzando = false;
        }
    }

    [RelayCommand]
    private async Task PenalizarAsync()
    {
        IsPenalizando = true;
        try
        {
            Penalizado = await _reportService.PenalizarAsync(ReporteId);
            await Toast.Make(Penalizado ? "Usuario penalizado." : "No se pudo penalizar.").Show();
        }
        finally
        {
            IsPenalizando = false;
        }
    }
}
