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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ConfigureHeroBanner(_userRole);
    }

    private void ConfigureHeroBanner(string role)
    {
        switch (role)
        {
            case "Aprendiz":
                HeroTitle.Text = "¿Algo no cuadra? ¡Repórtalo!";
                HeroDescription.Text = "Registra aquí cualquier novedad o situación que requiera atención";
                HeroCTA.Text = "Ir a Reportar";
                break;

            case "Administrador":
                HeroTitle.Text = "¿Algo pide acción? ¡Gestiónalo!";
                HeroDescription.Text = "Administra lo que pasa y gestiona los reportes de forma rápida y sencilla.";
                HeroCTA.Text = "Ir a Gestionar";
                break;

            default: // Particular
                HeroTitle.Text = "¿Eres aprendiz Sena?";
                HeroDescription.Text = "Como aprendiz puedes registrar cualquier novedad dentro y fuera del centro.";
                HeroCTA.Text = "Ingresar";
                break;
        }
    }

    private async void OnHeroCTATapped(object sender, EventArgs e)
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