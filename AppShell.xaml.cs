namespace EcosenaApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // register host route as main
            Routing.RegisterRoute("HostPage", typeof(Views.Host.HostPage));
        }
    }
}
