using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using POSRestaurant.Data;
using POSRestaurant.Pages;
using POSRestaurant.ViewModels;

namespace POSRestaurant
{
    /// <summary>
    /// Program startup class
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Program startup method
        /// </summary>
        /// <returns></returns>
        public static MauiApp CreateMauiApp()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseService>()
                .AddSingleton<HomeViewModel>()
                .AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
