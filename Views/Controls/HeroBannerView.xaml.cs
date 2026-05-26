namespace EcosenaApp.Views.Controls;

public partial class HeroBannerView : ContentView
{
    public static readonly BindableProperty HeroTitleTextProperty = BindableProperty.Create(
        nameof(HeroTitleText), typeof(string), typeof(HeroBannerView), "¿Eres aprendiz Sena?");

    public static readonly BindableProperty HeroDescriptionTextProperty = BindableProperty.Create(
        nameof(HeroDescriptionText), typeof(string), typeof(HeroBannerView), "Como aprendiz puedes registrar cualquier novedad dentro y fuera del centro.");

    public static readonly BindableProperty HeroCtaTextProperty = BindableProperty.Create(
        nameof(HeroCtaText), typeof(string), typeof(HeroBannerView), "Ingresar");

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

    public HeroBannerView()
    {
        InitializeComponent();
    }

    private void OnHeroCTATapped(object sender, EventArgs e)
    {
        HeroCtaClicked?.Invoke(this, EventArgs.Empty);
    }
}
