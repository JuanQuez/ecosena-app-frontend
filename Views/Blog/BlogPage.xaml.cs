namespace EcosenaApp.Views.Blog;

public partial class BlogPage : ContentPage
{
	public BlogPage()
	{
		InitializeComponent();
	}

    private async void OnBlogGoBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}