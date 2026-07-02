using EcosenaApp.ViewModels.Blog;

namespace EcosenaApp.Views.Blog;

public partial class BlogEntryPage : ContentPage
{
    private readonly BlogEntryViewModel? _viewModel;

    public BlogEntryPage(int id)
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<BlogEntryViewModel>();
        BindingContext = _viewModel;
        if (_viewModel != null)
            _viewModel.EntradaId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel == null)
            return;

        await _viewModel.LoadEntradaCommand.ExecuteAsync(null);
        var entrada = _viewModel.Entrada;
        if (entrada == null)
            return;

        TitleLabel.Text = entrada.Titulo;
        BodyLabel.Text = entrada.Contenido;
        AuthorLabel.Text = entrada.NombreRedactor;
        MainImage.Source = string.IsNullOrEmpty(entrada.Portada) ? "bkg_blog_cta.png" : entrada.Portada;
        AvatarImage.Source = string.IsNullOrEmpty(entrada.RedactorFoto) ? "avatar_autor.png" : entrada.RedactorFoto;

        AdminActions.IsVisible = _viewModel.IsAdmin;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // BlogEntryPage es la raíz de su propio NavigationPage (ver BlogContainerView.OnEntradaSelected);
        // hay que sacar ese NavigationPage completo de la pila de Shell, no la página local.
        await Shell.Current.Navigation.PopAsync();
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        await Navigation.PushAsync(new EditBlogEntryPage(_viewModel.EntradaId));
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (_viewModel == null)
            return;

        bool confirm = await DisplayAlert("Confirmar", "¿Eliminar esta entrada?", "Sí", "No");
        if (!confirm)
            return;

        await _viewModel.DeleteEntradaCommand.ExecuteAsync(null);
        if (_viewModel.Eliminado)
            await Shell.Current.Navigation.PopAsync();
    }
}