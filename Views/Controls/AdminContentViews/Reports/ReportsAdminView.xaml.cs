namespace EcosenaApp.Views.Controls;

public partial class ReportsAdminView : ContentView
{
    public event EventHandler? ReporteTapped;

    public ReportsAdminView()
    {
        InitializeComponent();
    }

    private void OnReporteTapped(object sender, TappedEventArgs e)
    {
        ReporteTapped?.Invoke(this, EventArgs.Empty);
    }
}
