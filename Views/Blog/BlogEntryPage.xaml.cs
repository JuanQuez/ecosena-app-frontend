using Microsoft.Maui.Controls;

namespace EcosenaApp.Views.Blog;

public partial class BlogEntryPage : ContentPage
{
    public BlogEntryPage(string title, string body, string author, string imageSource)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        BodyLabel.Text = body;
        AuthorLabel.Text = author;
        AvatarImage.Source = "avatar_autor.png";
        MainImage.Source = imageSource;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}