using CommunityToolkit.Maui.Behaviors;

namespace EcosenaApp.Views.Controls;

public partial class NewsTickerView : ContentView
{
    private static readonly string[] Titulares =
    {
        "Apulo se convierte en el escenario nacional del intercambio de conocimientos para transformar el campo",
        "Más de 67 mil colombianos están a un paso de convertirse en aprendices SENA",
        "El SENA impulsa la innovación y la transformación digital en la gestión documental del país",
        "En Cartagena de Indias se impulsan alianzas y conocimiento en PROCEMCO 2026, el gran encuentro del sector construcción"
    };

    private const double PixelsPerSecond = 40;
    private bool _animationStarted;

    public NewsTickerView()
    {
        InitializeComponent();

        AppendSequence();
        AppendSequence();

        TickerContent.SizeChanged += OnTickerContentSizeChanged;
    }

    private void AppendSequence()
    {
        foreach (var titular in Titulares)
        {
            TickerContent.Children.Add(new Label
            {
                Text = titular,
                FontFamily = "InterRegular",
                FontSize = 13,
                TextColor = (Color)Application.Current!.Resources["GrayDark"],
                LineBreakMode = LineBreakMode.NoWrap,
                VerticalOptions = LayoutOptions.Center
            });

            var separador = new Image
            {
                Source = "icon_sena_logo.svg",
                WidthRequest = 14,
                HeightRequest = 14,
                VerticalOptions = LayoutOptions.Center
            };
            separador.Behaviors.Add(new IconTintColorBehavior
            {
                TintColor = (Color)Application.Current!.Resources["GrayDark"]
            });
            TickerContent.Children.Add(separador);
        }
    }

    private void OnTickerContentSizeChanged(object? sender, EventArgs e)
    {
        if (_animationStarted || TickerContent.Children.Count < Titulares.Length * 4)
            return;

        var segundaSecuencia = (VisualElement)TickerContent.Children[Titulares.Length * 2];
        if (segundaSecuencia.X <= 0)
            return;

        _animationStarted = true;
        RunScrollAnimation(segundaSecuencia.X);
    }

    private void RunScrollAnimation(double segmentWidth)
    {
        if (segmentWidth <= 0)
            return;

        var duration = (uint)(segmentWidth / PixelsPerSecond * 1000);

        var animation = new Animation(v => TickerContent.TranslationX = v, 0, -segmentWidth);
        animation.Commit(this, "NewsTickerScroll", length: duration, easing: Easing.Linear,
            finished: (v, cancelled) =>
            {
                if (cancelled)
                    return;

                TickerContent.TranslationX = 0;
                RunScrollAnimation(segmentWidth);
            });
    }
}
