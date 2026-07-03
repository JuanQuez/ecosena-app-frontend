using CommunityToolkit.Maui.Alerts;

namespace EcosenaApp.Views.Controls;

public partial class OfflineBannerView : ContentView
{
    public OfflineBannerView()
    {
        InitializeComponent();

        IsVisible = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool sinInternet = e.NetworkAccess != NetworkAccess.Internet;

            if (!sinInternet && IsVisible)
            {
                IsVisible = false;
                await Toast.Make("Conexión restablecida.").Show();
                return;
            }

            IsVisible = sinInternet;
        });
    }
}
