using CommunityToolkit.Maui.Behaviors;

namespace EcosenaApp.Views.Controls;

public partial class FootBarView : ContentView
{
	public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
		nameof(SelectedIndex), typeof(int), typeof(FootBarView), 0, propertyChanged: OnSelectedIndexChanged);

	public int SelectedIndex
	{
		get => (int)GetValue(SelectedIndexProperty);
		set => SetValue(SelectedIndexProperty, value);
	}

	public event EventHandler<int>? SelectedIndexChanged;

	public FootBarView()
	{
		InitializeComponent();
		UpdateSelection(SelectedIndex);
	}

	private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (FootBarView)bindable;
		control.UpdateSelection((int)newValue);
		control.SelectedIndexChanged?.Invoke(control, (int)newValue);
	}

	private void OnHomeTapped(object sender, TappedEventArgs e) => SelectedIndex = 0;

	private void OnReportTapped(object sender, TappedEventArgs e) => SelectedIndex = 1;

	private void OnBlogTapped(object sender, TappedEventArgs e) => SelectedIndex = 2;

	private void UpdateSelection(int selectedIndex)
	{
		SetItemState(HomeContainer, HomeIcon, HomeLabel, selectedIndex == 0);
		SetItemState(ReportContainer, ReportIcon, ReportLabel, selectedIndex == 1);
		SetItemState(BlogContainer, BlogIcon, BlogLabel, selectedIndex == 2);
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
