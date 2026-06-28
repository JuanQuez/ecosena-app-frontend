namespace EcosenaApp.Views.Controls;

public partial class HeroUserView : ContentView
{
    public static readonly BindableProperty HeroTitleTextProperty = BindableProperty.Create(
        nameof(HeroTitleText), typeof(string), typeof(HeroUserView), "¿Algo no cuadra?\n¡Reportalo!");

    public static readonly BindableProperty HeroDescriptionTextProperty = BindableProperty.Create(
        nameof(HeroDescriptionText), typeof(string), typeof(HeroUserView), "Registra aquí cualquier novedad o situación que requiera atención");

    public static readonly BindableProperty HeroCtaTextProperty = BindableProperty.Create(
        nameof(HeroCtaText), typeof(string), typeof(HeroUserView), "Ir a Reportar");

    public string HeroTitleText
    {
        get => (string)GetValue(HeroTitleTextProperty);
        set => SetValue(HeroTitleTextProperty, value);
    }

    public string HeroDescriptionText
    {
        get => (string)GetValue(HeroDescriptionTextProperty);
        set => SetValue(HeroDescriptionTextProperty, value);
    }

    public string HeroCtaText
    {
        get => (string)GetValue(HeroCtaTextProperty);
        set => SetValue(HeroCtaTextProperty, value);
    }

    public event EventHandler? HeroCtaClicked;

    public HeroUserView()
    {
        InitializeComponent();
    }

    private void OnHeroCTATapped(object sender, EventArgs e)
    {
        HeroCtaClicked?.Invoke(this, EventArgs.Empty);
    }
}
