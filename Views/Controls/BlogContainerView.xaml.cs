using EcosenaApp.Models.Blog;
using EcosenaApp.ViewModels.Blog;

namespace EcosenaApp.Views.Controls
{
    public partial class BlogContainerView : ContentView
    {
        private readonly BlogViewModel? _viewModel;

        public BlogContainerView()
        {
            InitializeComponent();

            _viewModel = IPlatformApplication.Current?.Services.GetService<BlogViewModel>();
            BindingContext = _viewModel;
            NuevaEntradaButton.IsVisible = _viewModel?.IsAdmin ?? false;
            EntradasCollection.ItemsSource = _viewModel?.Entradas;

            if (_viewModel != null)
                _viewModel.LoadEntradasCommand.Execute(null);
        }

        public void Refresh() => _viewModel?.LoadEntradasCommand.Execute(null);

        private async void OnEntradaSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is BlogListResDto entrada)
            {
                EntradasCollection.SelectedItem = null;
                await Navigation.PushAsync(new Views.Blog.BlogEntryPage(entrada.Id));
            }
        }

        private async void OnNuevaEntradaTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Blog.CreateBlogEntryPage());
        }
    }
}
