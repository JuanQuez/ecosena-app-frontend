using Microsoft.Maui.Controls;

namespace EcosenaApp.Views.Controls
{
    public partial class BlogContainerView : ContentView
    {
        public BlogContainerView()
        {
            InitializeComponent();
        }

        private async void OnPostTapped(object sender, EventArgs e)
        {
            // Open sample entry page
            var title = "[TITULO DE ENTRADA]";
            var body = "BULLY is Ye's highly anticipated 12th studio album, and it's been one of the most delayed projects in recent memory — pushed back multiple times throughout 2025. He's now signed with independent music company Gamma for the release, and the album is currently slated for March 20, 2026. Sonically, Rolling Stone described it as drawing from the feel of 808s & Heartbreak and My Beautiful Dark Twisted Fantasy. Features include Peso Pluma, Playboy Carti, and Ty Dolla Sign.";
            var author = "Juan Skere";
            var image = "bkg_blog_cta.png";

            await Navigation.PushAsync(new Views.Blog.BlogEntryPage(title, body, author, image));
        }
    }
}
