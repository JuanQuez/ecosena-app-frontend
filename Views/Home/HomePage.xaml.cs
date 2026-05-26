using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosenaApp.Views.Home;

public partial class HomePage : ContentPage
{
    // Mock temporal hasta conectar con la API
    private readonly string _userRole = "Particular"; // "Particular" | "Aprendiz" | "Administrador"

    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnHeroCtaClicked(object sender, EventArgs e)
    {
        // Navegación temporal según rol
        switch (_userRole)
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
