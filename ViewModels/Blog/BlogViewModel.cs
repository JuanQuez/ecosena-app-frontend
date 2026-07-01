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

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isAdmin;

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
            var entradas = await _blogService.GetEntradasAsync();
            Entradas.Clear();
            foreach (var entrada in entradas)
                Entradas.Add(entrada);
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
}
