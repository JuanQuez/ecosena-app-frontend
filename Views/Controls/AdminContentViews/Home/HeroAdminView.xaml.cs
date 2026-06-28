namespace EcosenaApp.Views.Controls;

public partial class HeroAdminView : ContentView
{
    public static readonly BindableProperty HeroTitleTextProperty = BindableProperty.Create(
        nameof(HeroTitleText), typeof(string), typeof(HeroAdminView), "¿Algo pide acción?\n¡Gestionalo!");

    public static readonly BindableProperty HeroDescriptionTextProperty = BindableProperty.Create(
        nameof(HeroDescriptionText), typeof(string), typeof(HeroAdminView), "Gestiona y da seguimiento a reportes con facilidad.");

    public static readonly BindableProperty HeroCtaTextProperty = BindableProperty.Create(
        nameof(HeroCtaText), typeof(string), typeof(HeroAdminView), "Ir a Gestionar");

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

    public HeroAdminView()
    {
        InitializeComponent();
    }

    private void OnHeroCTATapped(object sender, EventArgs e)
    {
        HeroCtaClicked?.Invoke(this, EventArgs.Empty);
    }
}
