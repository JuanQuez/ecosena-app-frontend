using EcosenaApp.Services.Session;
using EcosenaApp.Views.Controls;

namespace EcosenaApp.Views.Host;

public partial class HostPage : ContentPage
{
    private readonly Dictionary<string, View> _cache = new();
    private readonly Stack<string> _history = new();
    private readonly IUserSession? _userSession;

    public HostPage()
    {
        InitializeComponent();

        _userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        MainFootBar.SetReportTabVisible(_userSession?.Role != "Invitado");

        // Wire up the footer selection
        MainFootBar.SelectedIndexChanged += OnFooterSelectionChanged;

        // Set initial view
        ShowSection("Home");
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
            "Report" => role switch
            {
                "Administrador" => new ReportsAdminView(),
                "Aprendiz" => new ReportsUserView(),
                "Penalizado" => BuildPenalizadoView(),
                _ => new ContentView(),
            },
            _ => new ContentView(),
        };
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
