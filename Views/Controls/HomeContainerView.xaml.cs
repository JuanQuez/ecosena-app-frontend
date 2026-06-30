using EcosenaApp.Services.Session;

namespace EcosenaApp.Views.Controls
{
    public partial class HomeContainerView : ContentView
    {
        public event EventHandler? ReportarRequested;

        public HomeContainerView()
        {
            InitializeComponent();
            BuildHero();
        }

        private void BuildHero()
        {
            var session = IPlatformApplication.Current?.Services.GetService<IUserSession>();
            var role = session?.Role ?? "Invitado";

            View hero = role switch
            {
                "Administrador" => CreateAdminHero(),
                "Aprendiz" => CreateUserHero(),
                "Penalizado" => CreateUserHero(),
                _ => CreateGuestHero(),
            };

            HeroSlot.Content = hero;
        }

        private View CreateAdminHero()
        {
            var hero = new HeroAdminView();
            hero.HeroCtaClicked += (s, e) => ReportarRequested?.Invoke(this, EventArgs.Empty);
            return hero;
        }

        private View CreateUserHero()
        {
            var hero = new HeroUserView();
            hero.HeroCtaClicked += (s, e) => ReportarRequested?.Invoke(this, EventArgs.Empty);
            return hero;
        }

        private View CreateGuestHero()
        {
            var hero = new HeroGuestView();
            hero.HeroCtaClicked += async (s, e) => await Shell.Current.GoToAsync("//LoginPage");
            return hero;
        }
    }
}
