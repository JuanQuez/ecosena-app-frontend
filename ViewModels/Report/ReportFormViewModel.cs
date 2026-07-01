using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Helpers;
using EcosenaApp.Services.Report;
using EcosenaApp.Services.Session;

namespace EcosenaApp.ViewModels.Report;

public partial class ReportFormViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    public List<(int Id, string Nombre)> Ambientes => AmbientesData.Lista;

    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int AmbienteIndex { get; set; } = -1;

    public Stream? FotoStream { get; private set; }
    public string? FotoFileName { get; private set; }

    [ObservableProperty]
    private ImageSource? fotoPreview;

    [ObservableProperty]
    private bool isBusy;

    public bool EsPenalizado { get; }

    public bool Enviado { get; private set; }

    public ReportFormViewModel(IReportService reportService, IUserSession userSession)
    {
        _reportService = reportService;
        EsPenalizado = userSession.Role == "Penalizado";
    }

    [RelayCommand]
    private async Task PickGaleriaAsync()
    {
        var resultado = await MediaPicker.PickPhotoAsync();
        await AplicarFotoAsync(resultado);
    }

    [RelayCommand]
    private async Task AbrirCamaraAsync()
    {
        var resultado = await MediaPicker.CapturePhotoAsync();
        await AplicarFotoAsync(resultado);
    }

    private async Task AplicarFotoAsync(FileResult? resultado)
    {
        if (resultado == null)
            return;

        FotoStream = await resultado.OpenReadAsync();
        FotoFileName = resultado.FileName;
        FotoPreview = ImageSource.FromStream(() => FotoStream);
    }

    [RelayCommand]
    private async Task EnviarReporteAsync()
    {
        Enviado = false;

        if (EsPenalizado)
        {
            await Toast.Make("Tu acceso a reportes está restringido.").Show();
            return;
        }

        if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Descripcion) || AmbienteIndex < 0)
        {
            await Toast.Make("Completa todos los campos.").Show();
            return;
        }

        IsBusy = true;
        var fotoAdjunta = FotoStream != null;
        try
        {
            var idAmbiente = Ambientes[AmbienteIndex].Id;
            var reporte = await _reportService.PostReportAsync(Titulo, Descripcion, idAmbiente, FotoStream, FotoFileName);
            Enviado = reporte != null;
            await Toast.Make(Enviado ? "Reporte enviado." : "No se pudo enviar el reporte.").Show();
        }
        finally
        {
            if (fotoAdjunta)
            {
                FotoStream = null;
                FotoFileName = null;
                FotoPreview = null;
            }
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        FotoStream = null;
        FotoFileName = null;
        FotoPreview = null;
        Titulo = string.Empty;
        Descripcion = string.Empty;
        AmbienteIndex = -1;
    }
}
