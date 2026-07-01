using EcosenaApp.ViewModels.Profile;

namespace EcosenaApp.Views.Profile;

public partial class EditProfilePage : ContentPage
{
    private readonly EditProfileViewModel? _viewModel;

    public EditProfilePage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<EditProfileViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
            await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnPickFotoTapped(object sender, EventArgs e)
    {
        if (_viewModel != null)
            await _viewModel.PickFotoCommand.ExecuteAsync(null);
    }
}
