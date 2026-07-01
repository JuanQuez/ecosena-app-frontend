using EcosenaApp.Helpers;
using EcosenaApp.ViewModels.Report;

namespace EcosenaApp.Views.Controls;

public partial class ReportsUserView : ContentView
{
    private readonly ReportsUserViewModel? _viewModel;
    private readonly ReportFormViewModel? _formViewModel;

    public ReportsUserView()
    {
        InitializeComponent();

        _viewModel = IPlatformApplication.Current?.Services.GetService<ReportsUserViewModel>();
        _formViewModel = IPlatformApplication.Current?.Services.GetService<ReportFormViewModel>();

        BindingContext = _viewModel;
        ReportesCollection.ItemsSource = _viewModel?.MisReportes;
        FormSection.BindingContext = _formViewModel;
        FormSection.SetAmbientes(AmbientesData.Lista.Select(a => a.Nombre));
        FormSection.SetBloqueado(_formViewModel?.EsPenalizado ?? false);

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.LoadCommand.Execute(null);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ReportsUserViewModel.MostrarFormulario) || _viewModel == null)
            return;

        ListSection.IsVisible = !_viewModel.MostrarFormulario;
        FormSection.IsVisible = _viewModel.MostrarFormulario;
    }

    private void OnReportarTapped(object sender, EventArgs e)
    {
        _viewModel?.AbrirFormularioCommand.Execute(null);
    }

    private async void OnGenerarReporteTapped(object? sender, EventArgs e)
    {
        if (_formViewModel == null)
            return;

        _formViewModel.Titulo = FormSection.Titulo;
        _formViewModel.Descripcion = FormSection.Descripcion;
        _formViewModel.AmbienteIndex = FormSection.AmbienteIndex;

        await _formViewModel.EnviarReporteCommand.ExecuteAsync(null);

        if (_formViewModel.Enviado)
        {
            _formViewModel?.CancelarCommand.Execute(null);
            FormSection.LimpiarFormulario();
            _viewModel?.VolverAListaCommand.Execute(null);
            if (_viewModel != null)
                await _viewModel.LoadCommand.ExecuteAsync(null);
        }
    }

    private void OnCancelarFormTapped(object? sender, EventArgs e)
    {
        _formViewModel?.CancelarCommand.Execute(null);
        FormSection.LimpiarFormulario();
        _viewModel?.VolverAListaCommand.Execute(null);
    }

    private async void OnAbrirGaleriaTapped(object? sender, EventArgs e)
    {
        if (_formViewModel == null)
            return;

        await _formViewModel.PickGaleriaCommand.ExecuteAsync(null);
        if (_formViewModel.FotoPreview != null)
            FormSection.SetFotoPreview(_formViewModel.FotoPreview);
    }

    private async void OnAbrirCamaraTapped(object? sender, EventArgs e)
    {
        if (_formViewModel == null)
            return;

        await _formViewModel.AbrirCamaraCommand.ExecuteAsync(null);
        if (_formViewModel.FotoPreview != null)
            FormSection.SetFotoPreview(_formViewModel.FotoPreview);
    }

    public void Refresh() => _viewModel?.LoadCommand.Execute(null);
}
