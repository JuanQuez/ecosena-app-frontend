namespace EcosenaApp.Views.Controls;

public partial class LoadingOverlayView : ContentView
{
    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(LoadingOverlayView), false, propertyChanged: OnIsBusyChanged);

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public LoadingOverlayView()
    {
        InitializeComponent();
    }

    private static void OnIsBusyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LoadingOverlayView)bindable;
        var busy = (bool)newValue;
        view.OverlayRoot.IsVisible = busy;
        view.Spinner.IsRunning = busy;
    }
}
