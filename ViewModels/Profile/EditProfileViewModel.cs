using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Services.Profile;

namespace EcosenaApp.ViewModels.Profile;

public partial class EditProfileViewModel : ObservableObject
{
    private readonly IProfileService _profileService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private DateTime fechaNacimiento = DateTime.Today;

    [ObservableProperty]
    private string nuevaContraseña = string.Empty;

    [ObservableProperty]
    private string confirmacionContraseña = string.Empty;

    [ObservableProperty]
    private ImageSource? fotoPreview;

    [ObservableProperty]
    private bool isBusy;

    public byte[]? FotoBytes { get; private set; }
    public string? FotoFileName { get; private set; }

    public bool Guardado { get; private set; }

    public EditProfileViewModel(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var profile = await _profileService.GetProfileAsync();
            if (profile == null)
                return;

            Email = profile.Email;
            if (DateTime.TryParse(profile.FechaNacimiento, out var fecha))
                FechaNacimiento = fecha;
            if (!string.IsNullOrEmpty(profile.FotoPerfil))
                FotoPreview = ImageSource.FromUri(new Uri(profile.FotoPerfil));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PickFotoAsync()
    {
        var accion = await Shell.Current.DisplayActionSheet("Foto de perfil", "Cancelar", null, "Galería", "Cámara");
        if (accion is not "Galería" and not "Cámara")
            return;

        var resultado = accion == "Galería"
            ? await MediaPicker.PickPhotoAsync()
            : await MediaPicker.CapturePhotoAsync();

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
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Toast.Make("El email no puede estar vacío.").Show();
            return;
        }

        if (!string.IsNullOrEmpty(NuevaContraseña) && NuevaContraseña != ConfirmacionContraseña)
        {
            await Toast.Make("Las contraseñas no coinciden.").Show();
            return;
        }

        IsBusy = true;
        var fotoAdjunta = FotoBytes != null;
        try
        {
            var contraseña = string.IsNullOrEmpty(NuevaContraseña) ? null : NuevaContraseña;
            var confirmacion = string.IsNullOrEmpty(NuevaContraseña) ? null : ConfirmacionContraseña;
            using var fotoParaEnviar = FotoBytes != null ? new MemoryStream(FotoBytes) : null;

            Guardado = await _profileService.UpdateProfileAsync(
                Email, DateOnly.FromDateTime(FechaNacimiento), contraseña, confirmacion, fotoParaEnviar, FotoFileName);

            await Toast.Make(Guardado ? "Perfil actualizado." : "No se pudo actualizar el perfil.").Show();
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
}
