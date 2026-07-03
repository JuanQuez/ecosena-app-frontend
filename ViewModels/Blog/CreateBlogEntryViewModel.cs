using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Blog;

namespace EcosenaApp.ViewModels.Blog;

public partial class CreateBlogEntryViewModel : ObservableObject
{
    private readonly IBlogService _blogService;

    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private ImageSource? portadaPreview;

    public bool IsNotBusy => !IsBusy;

    public byte[]? PortadaBytes { get; private set; }
    public string? PortadaFileName { get; private set; }

    public bool Publicado { get; private set; }

    public CreateBlogEntryViewModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [RelayCommand]
    private async Task PickPortadaAsync()
    {
        var accion = await Shell.Current.DisplayActionSheet("Portada", "Cancelar", null, "Galería", "Cámara");
        if (accion is not "Galería" and not "Cámara")
            return;

        FileResult? resultado;
        try
        {
            resultado = accion == "Galería"
                ? await MediaPicker.PickPhotoAsync()
                : await MediaPicker.CapturePhotoAsync();
        }
        catch (PermissionException)
        {
            await Toast.Make("Debes conceder permiso de cámara para tomar una foto.").Show();
            return;
        }
        catch (FeatureNotSupportedException)
        {
            await Toast.Make("Este dispositivo no tiene cámara disponible.").Show();
            return;
        }

        if (resultado == null)
            return;

        using var stream = await resultado.OpenReadAsync();
        using var memoria = new MemoryStream();
        await stream.CopyToAsync(memoria);

        PortadaBytes = memoria.ToArray();
        PortadaFileName = resultado.FileName;
        PortadaPreview = ImageSource.FromStream(() => new MemoryStream(PortadaBytes));
    }

    [RelayCommand]
    private async Task PublicarAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Contenido))
        {
            await Toast.Make("Completa título y contenido.").Show();
            return;
        }

        IsBusy = true;
        var fotoAdjunta = PortadaBytes != null;
        try
        {
            using var portadaParaEnviar = PortadaBytes != null ? new MemoryStream(PortadaBytes) : null;
            var entrada = await _blogService.PostEntradaAsync(Titulo, Contenido, portadaParaEnviar, PortadaFileName);
            Publicado = entrada != null;
            await Toast.Make(Publicado ? "Entrada publicada." : "No se pudo publicar la entrada.").Show();
        }
        finally
        {
            if (fotoAdjunta)
            {
                PortadaBytes = null;
                PortadaFileName = null;
                PortadaPreview = null;
            }
            IsBusy = false;
        }
    }
}
