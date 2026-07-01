using EcosenaApp.ViewModels.Blog;

namespace EcosenaApp.Views.Blog;

public partial class CreateBlogEntryPage : ContentPage
{
    private readonly CreateBlogEntryViewModel? _viewModel;

    public CreateBlogEntryPage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<CreateBlogEntryViewModel>();
        BindingContext = _viewModel;
    }

    private async void OnAbrirPortadaTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        await _viewModel.PickPortadaCommand.ExecuteAsync(null);
        if (_viewModel.PortadaPreview != null)
            CreateView.SetPortada(_viewModel.PortadaPreview);
    }

    private async void OnPublicarTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        _viewModel.Titulo = CreateView.Titulo;
        _viewModel.Contenido = CreateView.Contenido;
        await _viewModel.PublicarCommand.ExecuteAsync(null);

        if (_viewModel.Publicado)
            await Navigation.PopAsync();
    }

    private async void OnCancelarTapped(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
