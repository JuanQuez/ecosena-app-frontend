using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Blog;
using EcosenaApp.Services.Blog;
using EcosenaApp.Services.Session;

namespace EcosenaApp.ViewModels.Blog;

public partial class BlogEntryViewModel : ObservableObject
{
    private readonly IBlogService _blogService;

    public int EntradaId { get; set; }

    [ObservableProperty]
    private EntradaResDto? entrada;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstaProcesando))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstaProcesando))]
    private bool isEliminando;

    [ObservableProperty]
    private bool isAdmin;

    public bool EstaProcesando => IsBusy || IsEliminando;

    public bool Eliminado { get; private set; }

    public BlogEntryViewModel(IBlogService blogService, IUserSession userSession)
    {
        _blogService = blogService;
        IsAdmin = userSession.Role == "Administrador";
    }

    [RelayCommand]
    private async Task LoadEntradaAsync()
    {
        IsBusy = true;
        try
        {
            Entrada = await _blogService.GetEntradaAsync(EntradaId);
            if (Entrada == null)
                await Toast.Make("No se pudo cargar la entrada.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteEntradaAsync()
    {
        IsEliminando = true;
        try
        {
            Eliminado = await _blogService.DeleteEntradaAsync(EntradaId);
            await Toast.Make(Eliminado ? "Entrada eliminada." : "No se pudo eliminar.").Show();
        }
        finally
        {
            IsEliminando = false;
        }
    }
}
