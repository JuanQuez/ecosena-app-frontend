using EcosenaApp.ViewModels.Blog;

namespace EcosenaApp.Views.Blog;

public partial class EditBlogEntryPage : ContentPage
{
    private readonly EditBlogEntryViewModel? _viewModel;

    public EditBlogEntryPage(int id)
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<EditBlogEntryViewModel>();
        BindingContext = _viewModel;
        if (_viewModel != null)
            _viewModel.EntradaId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel == null)
            return;

        await _viewModel.LoadCommand.ExecuteAsync(null);
        EditView.CargarEntrada(_viewModel.Titulo, _viewModel.Contenido, _viewModel.PortadaPreview);
    }

    private async void OnCambiarPortadaTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        await _viewModel.PickPortadaCommand.ExecuteAsync(null);
        if (_viewModel.PortadaPreview != null)
            EditView.SetPortada(_viewModel.PortadaPreview);
    }

    private async void OnGuardarTapped(object? sender, EventArgs e)
    {
        if (_viewModel == null || _viewModel.IsGuardando)
            return;

        _viewModel.Titulo = EditView.Titulo;
        _viewModel.Contenido = EditView.Contenido;
        await _viewModel.GuardarCommand.ExecuteAsync(null);

        if (_viewModel.Guardado)
            await Navigation.PopAsync();
    }

    private async void OnCancelarTapped(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
