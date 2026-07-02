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

    public byte[]? FotoBytes { get; private set; }
    public string? FotoFileName { get; private set; }

    [ObservableProperty]
    private ImageSource? fotoPreview;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

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

        using var stream = await resultado.OpenReadAsync();
        using var memoria = new MemoryStream();
        await stream.CopyToAsync(memoria);

        FotoBytes = memoria.ToArray();
        FotoFileName = resultado.FileName;
        FotoPreview = ImageSource.FromStream(() => new MemoryStream(FotoBytes));
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
        var fotoAdjunta = FotoBytes != null;
        try
        {
            var idAmbiente = Ambientes[AmbienteIndex].Id;
            using var fotoParaEnviar = FotoBytes != null ? new MemoryStream(FotoBytes) : null;
            var reporte = await _reportService.PostReportAsync(Titulo, Descripcion, idAmbiente, fotoParaEnviar, FotoFileName);
            Enviado = reporte != null;
            await Toast.Make(Enviado ? "Reporte enviado." : "No se pudo enviar el reporte.").Show();
        }
        finally
        {
            if (fotoAdjunta)
            {
                FotoBytes = null;
                FotoFileName = null;
                FotoPreview = null;
            }
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        FotoBytes = null;
        FotoFileName = null;
        FotoPreview = null;
        Titulo = string.Empty;
        Descripcion = string.Empty;
        AmbienteIndex = -1;
    }
}
