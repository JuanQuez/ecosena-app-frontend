using EcosenaApp.Helpers;

namespace EcosenaApp.Views.Controls
{
    public partial class InterestInfoView : ContentView
    {
        private const string InstructivoUrl = "https://drive.google.com/file/d/17eGIrP5z2ztvBDoQvNlfjcX18_wtUHi1/view?usp=sharing";

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

        private async void OnVerInstructivoTapped(object sender, EventArgs e)
        {
            await Browser.Default.OpenAsync(new Uri(InstructivoUrl), BrowserLaunchMode.SystemPreferred);
        }
    }
}
