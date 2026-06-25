using Microsoft.Maui.Controls;

namespace EcosenaApp.Views.Controls
{
    public partial class HomeContainerView : ContentView
    {
        public HomeContainerView()
        {
            InitializeComponent();
        }

        private async void OnHeroCtaClicked(object sender, EventArgs e)
        {
            // Navegación temporal según rol
            var userRole = "Particular"; // Mock temporal

            switch (userRole)
            {
                case "Aprendiz":
                    // await Shell.Current.GoToAsync("//ReportPage");
                    break;
                case "Administrador":
                    // await Shell.Current.GoToAsync("//ManagePage");
                    break;
                default:
                    await Shell.Current.GoToAsync("//LoginPage");
                    break;
            }
        }
    }
}
