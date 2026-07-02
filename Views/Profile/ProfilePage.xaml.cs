using EcosenaApp.ViewModels.Profile;

namespace EcosenaApp.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private const string SoporteEmail = "juanquez.dev@gmail.com";

    private const string PrivacidadTexto =
        "En EcoSENA recopilamos únicamente los datos necesarios para operar la aplicación: tu documento, nombre, " +
        "programa de formación, ficha y correo, junto con la información de los reportes ambientales que registras " +
        "(ubicación, descripción y foto).\n\n" +
        "Esta información se usa exclusivamente para gestionar tu cuenta, dar seguimiento a los reportes ante el " +
        "personal del SENA encargado y mostrarte el estado de tus propias solicitudes. No compartimos tus datos " +
        "con terceros ajenos al proceso de gestión ambiental institucional.\n\n" +
        "Puedes solicitar la actualización o eliminación de tu información escribiendo a través de la opción " +
        "\"Reportar un problema\" en este mismo menú.";

    private const string TerminosTexto =
        "Al usar EcoSENA aceptas registrar reportes ambientales veraces y relacionados con las instalaciones del " +
        "SENA. El uso indebido de la herramienta, incluyendo el registro de reportes falsos o repetidos sin " +
        "fundamento, puede derivar en la penalización de tu cuenta por parte de un administrador, restringiendo " +
        "temporalmente tu capacidad de crear nuevos reportes.\n\n" +
        "El contenido del blog educativo es publicado únicamente por administradores y tiene fines informativos " +
        "sobre buenas prácticas ambientales dentro del SENA.\n\n" +
        "EcoSENA es una herramienta interna de apoyo a la gestión ambiental institucional y no reemplaza los " +
        "canales oficiales de emergencia ante incidentes graves.";

    private const string NotificacionesTexto =
        "EcoSENA no envía notificaciones push a tu dispositivo. En su lugar, cada vez que abres la sección " +
        "\"Reportar\" la aplicación revisa el estado de tus reportes y te muestra un aviso en pantalla si alguno " +
        "cambió desde la última vez que la revisaste (por ejemplo, de \"Pendiente\" a \"En progreso\").\n\n" +
        "Para ver el estado más reciente de tus reportes, simplemente vuelve a abrir esa sección.";

    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetService<ProfileViewModel>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
            vm.LoadProfileCommand.Execute(null);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // ProfilePage es la raíz de su propio NavigationPage (ver TopBarView.OnProfileTapped);
        // hay que sacar ese NavigationPage completo de la pila de Shell, no la página local.
        await Shell.Current.Navigation.PopAsync();
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");
        if (confirm && BindingContext is ProfileViewModel vm)
            vm.LogoutCommand.Execute(null);
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage());
    }

    private async void OnNotificationsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InfoTextPage("Notificaciones", NotificacionesTexto));
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InfoTextPage("Privacidad", PrivacidadTexto));
    }

    private async void OnReportProblemTapped(object sender, EventArgs e)
    {
        var asunto = Uri.EscapeDataString("Reporte de problema - EcoSENA");
        var cuerpo = Uri.EscapeDataString(
            $"Describe el problema encontrado:\n\n\n" +
            $"---\n" +
            $"Versión de app: {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})\n" +
            $"Plataforma: {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString}\n" +
            $"Dispositivo: {DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}");

        try
        {
            await Launcher.Default.OpenAsync(new Uri($"mailto:{SoporteEmail}?subject={asunto}&body={cuerpo}"));
        }
        catch (Exception)
        {
            await DisplayAlert("Sin app de correo", $"Escríbenos a {SoporteEmail} para reportar el problema.", "OK");
        }
    }

    private async void OnTermsConditionsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InfoTextPage("Términos y condiciones", TerminosTexto));
    }
}
