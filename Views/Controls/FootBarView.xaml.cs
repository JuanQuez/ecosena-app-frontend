using CommunityToolkit.Maui.Behaviors;

namespace EcosenaApp.Views.Controls;

public partial class FootBarView : ContentView
{
	private const int HomeIndex = 0;
	private const int ReportIndex = 1;
	private const int BlogIndex = 2;
	private const string HomeRoute = "//HomePage";
	private const string BlogRoute = "//BlogPage";

	public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
		nameof(SelectedIndex), typeof(int), typeof(FootBarView), HomeIndex, propertyChanged: OnSelectedIndexChanged);

	public int SelectedIndex
	{
		get => (int)GetValue(SelectedIndexProperty);
		set => SetValue(SelectedIndexProperty, value);
	}

	public event EventHandler<int>? SelectedIndexChanged;

	public FootBarView()
	{
		InitializeComponent();
		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
		SyncSelectionWithCurrentRoute();
	}

	private void OnLoaded(object? sender, EventArgs e)
	{
		if (Shell.Current is not null)
		{
			Shell.Current.Navigated += OnShellNavigated;
		}

		SyncSelectionWithCurrentRoute();
	}

	private void OnUnloaded(object? sender, EventArgs e)
	{
		if (Shell.Current is not null)
		{
			Shell.Current.Navigated -= OnShellNavigated;
		}
	}

	private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		SyncSelectionWithCurrentRoute();
	}

	private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (FootBarView)bindable;
		control.UpdateSelection((int)newValue);
		control.SelectedIndexChanged?.Invoke(control, (int)newValue);
	}

	private void OnHomeTapped(object sender, TappedEventArgs e) => NavigateTo(HomeIndex, HomeRoute);

	private void OnReportTapped(object sender, TappedEventArgs e) => NavigateTo(ReportIndex, null);

	private void OnBlogTapped(object sender, TappedEventArgs e) => NavigateTo(BlogIndex, BlogRoute);

	private async void NavigateTo(int index, string? route)
	{
		if (SelectedIndex != index)
		{
			SelectedIndex = index;
		}

		if (!string.IsNullOrWhiteSpace(route) && Shell.Current is not null)
		{
			await Shell.Current.GoToAsync(route);
		}
	}

	private void SyncSelectionWithCurrentRoute()
	{
		var route = Shell.Current?.CurrentState.Location.ToString() ?? string.Empty;
		var targetIndex = GetIndexForRoute(route);

		if (targetIndex >= 0 && SelectedIndex != targetIndex)
		{
			SelectedIndex = targetIndex;
		}
	}

	private static int GetIndexForRoute(string route)
	{
		if (route.Contains("BlogPage", StringComparison.OrdinalIgnoreCase))
		{
			return BlogIndex;
		}

		if (route.Contains("HomePage", StringComparison.OrdinalIgnoreCase))
		{
			return HomeIndex;
		}

		return -1;
	}

	private void UpdateSelection(int selectedIndex)
	{
		SetItemState(HomeContainer, HomeIcon, HomeLabel, selectedIndex == HomeIndex);
		SetItemState(ReportContainer, ReportIcon, ReportLabel, selectedIndex == ReportIndex);
		SetItemState(BlogContainer, BlogIcon, BlogLabel, selectedIndex == BlogIndex);
	}

	private static void SetItemState(Border container, Image icon, Label label, bool isActive)
	{
		var resources = Application.Current?.Resources;
		var activeBackground = resources != null ? (Brush)resources["InnerShadowBtn"] : Brush.Transparent;
		var activeText = resources != null ? (Color)resources["Fantasy"] : Colors.White;
		var inactiveText = resources != null ? (Color)resources["GrayLight"] : Colors.LightGray;

		container.Background = isActive ? activeBackground : Brush.Transparent;
		label.TextColor = isActive ? activeText : inactiveText;

		var tintBehavior = icon.Behaviors.OfType<IconTintColorBehavior>().FirstOrDefault();
		if (tintBehavior is not null)
		{
			tintBehavior.TintColor = isActive ? activeText : inactiveText;
		}
	}
}
