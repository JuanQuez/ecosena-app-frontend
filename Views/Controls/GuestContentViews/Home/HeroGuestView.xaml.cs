namespace EcosenaApp.Views.Controls;

public partial class HeroGuestView : ContentView
{
    public static readonly BindableProperty HeroTitleTextProperty = BindableProperty.Create(
        nameof(HeroTitleText), typeof(string), typeof(HeroGuestView), "¿Eres aprendiz Sena?");

    public static readonly BindableProperty HeroDescriptionTextProperty = BindableProperty.Create(
        nameof(HeroDescriptionText), typeof(string), typeof(HeroGuestView), "Como aprendiz puedes registrar cualquier novedad dentro y fuera del centro.");

    public static readonly BindableProperty HeroCtaTextProperty = BindableProperty.Create(
        nameof(HeroCtaText), typeof(string), typeof(HeroGuestView), "Ingresar");

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

    public HeroGuestView()
    {
        InitializeComponent();
    }

    private void OnHeroCTATapped(object sender, EventArgs e)
    {
        HeroCtaClicked?.Invoke(this, EventArgs.Empty);
    }
}
