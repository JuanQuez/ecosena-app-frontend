using EcosenaApp.Services.Session;
using EcosenaApp.Views.Controls;

namespace EcosenaApp.Views.Host;

public partial class HostPage : ContentPage
{
    private readonly Dictionary<string, View> _cache = new();
    private readonly Stack<string> _history = new();
    private readonly IUserSession? _userSession;
    private string? _lastRole;

    public HostPage()
    {
        InitializeComponent();

        _userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();

        // Wire up the footer selection
        MainFootBar.SelectedIndexChanged += OnFooterSelectionChanged;

        // Al recuperar conexión, refresca la sección activa (mismo mecanismo que RefreshIfSupported usa al volver de background)
        OfflineBanner.ConnectivityRestored += (s, e) =>
        {
            if (ContentRegion.Content is View currentView)
                RefreshIfSupported(currentView);
        };

        ApplyRole();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = MainTopBar.RefreshProfilePhotoAsync();

        var role = _userSession?.Role ?? "Invitado";
        if (role != _lastRole)
        {
            ApplyRole();
        }
        else if (ContentRegion.Content is View currentView)
        {
            RefreshIfSupported(currentView);
        }
    }

    private void ApplyRole()
    {
        var role = _userSession?.Role ?? "Invitado";

        MainFootBar.SetReportTabVisible(role != "Invitado");
        _cache.Clear();
        _history.Clear();

        ShowSection("Home");

        _lastRole = role;
    }

    private void OnFooterSelectionChanged(object? sender, int selectedIndex)
    {
        string section = selectedIndex switch
        {
            0 => "Home",
            1 => "Report",
            2 => "Blog",
            _ => "Home"
        };

        ShowSection(section);
    }

    public void ShowSection(string key)
    {
        if (_cache.TryGetValue(key, out var view))
        {
            ContentRegion.Content = view;
            RefreshIfSupported(view);
        }
        else
        {
            view = CreateViewForKey(key);
            _cache[key] = view;
            ContentRegion.Content = view;
        }

        // push to history
        if (_history.Count == 0 || _history.Peek() != key)
        {
            _history.Push(key);
        }

        // update footer selected index
        if (MainFootBar != null)
        {
            MainFootBar.SelectedIndex = key == "Home" ? 0 : (key == "Report" ? 1 : 2);
        }
    }

    private View CreateViewForKey(string key)
    {
        var role = _userSession?.Role ?? "Invitado";

        return key switch
        {
            "Home" => CreateHomeView(),
            "Blog" => new BlogContainerView(),
            "Report" => CreateReportView(role),
            _ => new ContentView(),
        };
    }

    private View CreateReportView(string role)
    {
        switch (role)
        {
            case "Administrador":
                return new ReportsAdminView();
            case "Aprendiz":
                var reportsUserView = new ReportsUserView();
                reportsUserView.BusyChanged += (s, busy) => HostLoadingOverlay.IsBusy = busy;
                return reportsUserView;
            case "Penalizado":
                return BuildPenalizadoView();
            default:
                return new ContentView();
        }
    }

    private View CreateHomeView()
    {
        var view = new HomeContainerView();
        view.ReportarRequested += (s, e) => ShowSection("Report");
        return view;
    }

    private static View BuildPenalizadoView()
    {
        return new ContentView
        {
            Content = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Padding = 30,
                Children =
                {
                    new Label
                    {
                        Text = "Tu acceso a reportes está restringido.",
                        Style = (Style)Application.Current!.Resources["H3"],
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = (Color)Application.Current!.Resources["GrayDark"]
                    }
                }
            }
        };
    }

    private static void RefreshIfSupported(View view)
    {
        switch (view)
        {
            case BlogContainerView blogView:
                blogView.Refresh();
                break;
            case ReportsAdminView reportsAdminView:
                reportsAdminView.Refresh();
                break;
            case ReportsUserView reportsUserView:
                reportsUserView.Refresh();
                break;
            case HomeContainerView homeView:
                homeView.Refresh();
                break;
        }
    }

    public bool TryGoBack()
    {
        if (_history.Count > 1)
        {
            // pop current
            _history.Pop();
            var previous = _history.Peek();
            if (_cache.TryGetValue(previous, out var view))
            {
                ContentRegion.Content = view;
            }
            return true;
        }
        return false;
    }
}
