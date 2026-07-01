using EcosenaApp.ViewModels.Auth;

namespace EcosenaApp.Views.Auth;

public partial class ForgotPassPage : ContentPage
{
    private readonly ForgotPassViewModel? _viewModel;

    public ForgotPassPage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<ForgotPassViewModel>();
        BindingContext = _viewModel;

        if (_viewModel != null)
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        Paso1Section.IsVisible = true;
        Paso2Section.IsVisible = false;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ForgotPassViewModel.EnSegundoPaso) || _viewModel == null)
            return;

        Paso1Section.IsVisible = !_viewModel.EnSegundoPaso;
        Paso2Section.IsVisible = _viewModel.EnSegundoPaso;
    }

    private async void OnLoginGoBcak(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
