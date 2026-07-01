using EcosenaApp.Models.Blog;
using EcosenaApp.Services.Blog;

namespace EcosenaApp.Views.Controls
{
    public partial class RecentBlogView : ContentView
    {
        private readonly IBlogService? _blogService;
        private BlogListResDto? _entradaReciente;

        public RecentBlogView()
        {
            InitializeComponent();
            _blogService = IPlatformApplication.Current?.Services.GetService<IBlogService>();
            _ = LoadRecienteAsync();
        }

        private async Task LoadRecienteAsync()
        {
            if (_blogService == null)
                return;

            var entradas = await _blogService.GetEntradasAsync();
            _entradaReciente = entradas.OrderByDescending(e => e.FechaPublicacion).FirstOrDefault();

            if (_entradaReciente == null)
            {
                EntradaCard.IsVisible = false;
                SinEntradasLabel.IsVisible = true;
                return;
            }

            AutorLabel.Text = _entradaReciente.NombreRedactor;
            TituloLabel.Text = _entradaReciente.Titulo;
            FechaLabel.Text = _entradaReciente.FechaPublicacion.ToString("d MMM yyyy");
        }

        private async void OnEntradaTapped(object sender, TappedEventArgs e)
        {
            if (_entradaReciente == null)
                return;

            // Se envuelve en un NavigationPage propio para aislar el push/pop de BlogEntryPage
            // de la pila de Shell (mismo patrón que BlogContainerView.OnEntradaSelected).
            var entradaPage = new Views.Blog.BlogEntryPage(_entradaReciente.Id);
            NavigationPage.SetHasNavigationBar(entradaPage, false);
            await Navigation.PushAsync(new NavigationPage(entradaPage));
        }

        private void OnViewAllTapped(object sender, TappedEventArgs e)
        {
            // Find the MainFootBar in the parent HostPage and update selection
            var page = Application.Current?.MainPage as Shell;
            if (page?.CurrentPage is Views.Host.HostPage hostPage)
            {
                hostPage.ShowSection("Blog");
            }
        }
    }
}
