using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcosenaApp.Models.Blog;
using EcosenaApp.Services.Blog;
using EcosenaApp.Services.Session;
using System.Collections.ObjectModel;

namespace EcosenaApp.ViewModels.Blog;

public partial class BlogViewModel : ObservableObject
{
    private readonly IBlogService _blogService;
    private List<BlogListResDto> _todasLasEntradas = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isAdmin;

    [ObservableProperty]
    private string busqueda = string.Empty;

    public ObservableCollection<BlogListResDto> Entradas { get; } = new();

    public BlogViewModel(IBlogService blogService, IUserSession userSession)
    {
        _blogService = blogService;
        IsAdmin = userSession.Role == "Administrador";
    }

    [RelayCommand]
    private async Task LoadEntradasAsync()
    {
        IsBusy = true;
        try
        {
            _todasLasEntradas = await _blogService.GetEntradasAsync();
            AplicarFiltro();
        }
        catch (Exception)
        {
            await Toast.Make("No se pudieron cargar las entradas.").Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnBusquedaChanged(string value) => AplicarFiltro();

    private void AplicarFiltro()
    {
        var filtradas = string.IsNullOrWhiteSpace(Busqueda)
            ? _todasLasEntradas
            : _todasLasEntradas.Where(e => e.Titulo.Contains(Busqueda, StringComparison.OrdinalIgnoreCase)).ToList();

        Entradas.Clear();
        foreach (var entrada in filtradas)
            Entradas.Add(entrada);
    }
}
