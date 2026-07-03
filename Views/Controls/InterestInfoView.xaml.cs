using EcosenaApp.Helpers;

namespace EcosenaApp.Views.Controls
{
    public partial class InterestInfoView : ContentView
    {
        public InterestInfoView()
        {
            InitializeComponent();
            NoticiasCollection.ItemsSource = SenaNoticiasData.Lista;
        }

        private async void OnNoticiaSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is SenaNoticia noticia)
            {
                NoticiasCollection.SelectedItem = null;
                await Browser.Default.OpenAsync(new Uri(noticia.Url), BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}
