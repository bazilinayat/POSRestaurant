using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using POSRestaurant.DBO;
using POSRestaurant.Pages;
using POSRestaurant.Service;
using POSRestaurant.Utility;
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
                .AddSingleton<SettingService>()
                .AddSingleton<TaxService>()
                .AddSingleton<MenuService>()
                
                .AddSingleton<MainPage>()
                .AddSingleton<OrdersPage>()
                .AddSingleton<TablePage>()
                .AddSingleton<ManageMenuItemPage>()
                .AddSingleton<OrderViewPage>()
                .AddSingleton<StaffManagementPage>()
                .AddSingleton<BillPage>()
                .AddSingleton<InventoryPage>()
                .AddSingleton<ItemReportPage>()
                .AddSingleton<ExpenseItemManagementPage>()
                
                .AddSingleton<ShellViewModel>()
                .AddSingleton<HomeViewModel>()
                .AddSingleton<OrdersViewModel>()
                .AddSingleton<TableViewModel>()
                .AddSingleton<ManageMenuItemViewModel>()
                .AddSingleton<ViewOrderViewModel>()
                .AddSingleton<StaffManagementViewModel>()
                .AddSingleton<BillViewModel>()
                .AddSingleton<OrderCompleteViewModel>()
                .AddSingleton<InventoryViewModel>()
                .AddSingleton<ItemReportViewModel>()
                .AddSingleton<ExpenseItemViewModel>();

            return builder.Build();
        }
    }
}
