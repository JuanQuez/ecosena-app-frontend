using EcosenaApp.Services.Auth;
using EcosenaApp.Services.Session;

namespace EcosenaApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var authService = IPlatformApplication.Current?.Services.GetService<IAuthService>();
            var userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
            if (authService is null || userSession is null)
                return;

            var token = await authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return;

            userSession.SetFromToken(token);
            if (Shell.Current is not null)
                await Shell.Current.GoToAsync("//HostPage");
        }
    }
}