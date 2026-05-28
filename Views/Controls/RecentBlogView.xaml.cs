using Microsoft.Maui.Controls;

namespace EcosenaApp.Views.Controls
{
    public partial class RecentBlogView : ContentView
    {
        public RecentBlogView()
        {
            InitializeComponent();
        }

        private async void OnViewAllTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//BlogPage");
        }
    }
}
