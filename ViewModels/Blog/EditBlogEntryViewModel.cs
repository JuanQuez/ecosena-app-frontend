using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Blog;

namespace EcosenaApp.ViewModels.Blog;

public partial class EditBlogEntryViewModel : ObservableObject
{
    private readonly IBlogService _blogService;

    public int EntradaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private ImageSource? portadaPreview;

    public Stream? PortadaStream { get; private set; }
    public string? PortadaFileName { get; private set; }

    public bool Guardado { get; private set; }

    public EditBlogEntryViewModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var entrada = await _blogService.GetEntradaAsync(EntradaId);
            if (entrada == null)
            {
                await Toast.Make("No se pudo cargar la entrada.").Show();
                return;
            }

            Titulo = entrada.Titulo;
            Contenido = entrada.Contenido;
            if (!string.IsNullOrEmpty(entrada.Portada))
                PortadaPreview = ImageSource.FromUri(new Uri(entrada.Portada));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PickPortadaAsync()
    {
        var accion = await Shell.Current.DisplayActionSheet("Portada", "Cancelar", null, "Galería", "Cámara");
        if (accion is not "Galería" and not "Cámara")
            return;

        var resultado = accion == "Galería"
            ? await MediaPicker.PickPhotoAsync()
            : await MediaPicker.CapturePhotoAsync();

        if (resultado == null)
            return;

        PortadaStream = await resultado.OpenReadAsync();
        PortadaFileName = resultado.FileName;
        PortadaPreview = ImageSource.FromStream(() => PortadaStream);
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Contenido))
        {
            await Toast.Make("Completa título y contenido.").Show();
            return;
        }

        IsBusy = true;
        try
        {
            Guardado = await _blogService.PutEntradaAsync(EntradaId, Titulo, Contenido, PortadaStream, PortadaFileName);
            await Toast.Make(Guardado ? "Cambios guardados." : "No se pudo guardar.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
