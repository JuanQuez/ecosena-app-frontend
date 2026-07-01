using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using EcosenaApp.Services.Auth;
using EcosenaApp.Services.Blog;
using EcosenaApp.Services.Profile;
using EcosenaApp.Services.Report;
using EcosenaApp.Services.Session;
using EcosenaApp.ViewModels.Auth;
using EcosenaApp.ViewModels.Blog;
using EcosenaApp.ViewModels.Profile;
using EcosenaApp.ViewModels.Report;

namespace EcosenaApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter_18pt-Light.ttf", "InterLight");
                    fonts.AddFont("Inter_18pt-Medium.ttf", "InterMedium");
                    fonts.AddFont("Inter_18pt-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter_18pt-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter_18pt-Bold", "InterBold");
                    fonts.AddFont("Inter_18pt-ExtraBold.ttf", "InterExtraBold");
                });

            // Register services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IBlogService, BlogService>();
            builder.Services.AddSingleton<IProfileService, ProfileService>();
            builder.Services.AddSingleton<IReportService, ReportService>();
            builder.Services.AddSingleton<IUserSession, UserSession>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<EditProfileViewModel>();
            builder.Services.AddTransient<BlogViewModel>();
            builder.Services.AddTransient<CreateBlogEntryViewModel>();
            builder.Services.AddTransient<EditBlogEntryViewModel>();
            builder.Services.AddTransient<BlogEntryViewModel>();
            builder.Services.AddTransient<ReportsUserViewModel>();
            builder.Services.AddTransient<ReportFormViewModel>();
            builder.Services.AddTransient<ReportManagementViewModel>();
            builder.Services.AddTransient<ReportsAdminViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
