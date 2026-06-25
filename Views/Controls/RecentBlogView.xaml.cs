using Microsoft.Maui.Controls;

namespace EcosenaApp.Views.Controls
{
    public partial class RecentBlogView : ContentView
    {
        public RecentBlogView()
        {
            InitializeComponent();
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
