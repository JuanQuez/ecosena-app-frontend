namespace EcosenaApp.Views.Controls;

public partial class ReportsUserView : ContentView
{
    public event EventHandler? ReportarClicked;

    public ReportsUserView()
    {
        InitializeComponent();
    }

    private void OnReportarTapped(object sender, EventArgs e)
    {
        ReportarClicked?.Invoke(this, EventArgs.Empty);
    }
}
