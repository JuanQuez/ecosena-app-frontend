using EcosenaApp.Views.Controls;

namespace EcosenaApp.Views.Host;

public partial class HostPage : ContentPage
{
    private readonly Dictionary<string, View> _cache = new();
    private readonly Stack<string> _history = new();

    public HostPage()
    {
        InitializeComponent();

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
        return key switch
        {
            "Home" => new HomeContainerView(),
            "Blog" => new BlogContainerView(),
            "Report" => new ContentView(),
            _ => new ContentView(),
        };
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