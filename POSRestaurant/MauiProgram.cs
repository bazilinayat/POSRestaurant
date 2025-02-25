using CommunityToolkit.Maui;
using POSRestaurant.Service.LoggerService;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using POSRestaurant.PaymentService.Online;
using POSRestaurant.DBO;
using POSRestaurant.Pages;
using POSRestaurant.Service;
using POSRestaurant.ViewModels;
using POSRestaurant.Service.SettingService;

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
            string logFilePath = Path.Combine(AppContext.BaseDirectory, "logs");

            builder.Services.AddSingleton(new LogService(logFilePath));

            builder.Services.AddSingleton<DatabaseService>()
                .AddSingleton<SettingService>()
                .AddSingleton<TaxService>()
                .AddSingleton<MenuService>()
                .AddSingleton<PaytmService>()
                .AddSingleton<ReceiptService>()

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
                .AddSingleton<InventoryReport>()
                .AddSingleton<SettingsPage>()
                .AddSingleton<PickupPage>()
                .AddSingleton<SalesReportPage>()
                
                .AddSingleton<ShellViewModel>()
                .AddSingleton<HomeViewModel>()
                .AddSingleton<OrdersViewModel>()
                .AddSingleton<TableViewModel>()
                .AddSingleton<ManageMenuItemViewModel>()
                .AddSingleton<OrderViewViewModel>()
                .AddSingleton<StaffManagementViewModel>()
                .AddSingleton<BillViewModel>()
                .AddSingleton<OrderCompleteViewModel>()
                .AddSingleton<InventoryViewModel>()
                .AddSingleton<ItemReportViewModel>()
                .AddSingleton<ExpenseItemViewModel>()
                .AddSingleton<InventoryReportViewModel>()
                .AddSingleton<SettingsViewModel>()
                .AddSingleton<PickupViewModel>()
                .AddSingleton<SalesReportViewModel>();

            // Force initialize Windows App Runtime components
            if (OperatingSystem.IsWindows())
            {
                builder.ConfigureLifecycleEvents(events =>
                {
                    events.AddWindows(windows => windows
                        .OnWindowCreated(window =>
                        {
                            window.ExtendsContentIntoTitleBar = false;
                        })
                    );
                });
            }

            return builder.Build();
        }
    }
}
