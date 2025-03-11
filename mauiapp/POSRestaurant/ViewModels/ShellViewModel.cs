using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Controls;
using POSRestaurant.DBO;
using POSRestaurant.Pages;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;

namespace POSRestaurant.ViewModels
{
    public partial class ShellViewModel : ObservableObject, IRecipient<TaxChangedMessage>
    {
        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// The name of the restaurant
        /// </summary>
        [ObservableProperty]
        private string _restaurantName;

        /// <summary>
        /// Constructor for the TablesViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public ShellViewModel(IServiceProvider serviceProvider, LogService logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.Register<TaxChangedMessage>(this);

            GetRestaurantName();
        }

        /// <summary>
        /// Just to reload the application name
        /// </summary>
        /// <param name="message">TaxChangedMessage</param>
        public async void Receive(TaxChangedMessage message)
        {
            try
            {
                var databaseService = _serviceProvider.GetRequiredService<DatabaseService>();
                var resInfo = await databaseService.SettingsOperation.GetRestaurantInfo();
                if (resInfo != null)
                {
                    RestaurantName = resInfo.Name;
                }
                else
                {
                    RestaurantName = "Restaurant Name";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-Receive TaxChangedMessage Error", ex);
            }
        }

        /// <summary>
        /// To get the restaurant name
        /// </summary>
        /// <returns></returns>
        private async Task GetRestaurantName()
        {
            try
            {
                var databaseService = _serviceProvider.GetRequiredService<DatabaseService>();
                var resInfo = await databaseService.SettingsOperation.GetRestaurantInfo();
                if (resInfo != null)
                {
                    RestaurantName = resInfo.Name;
                }
                else
                {
                    RestaurantName = "Restaurant Name";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-GetRestaurantName Error", ex);
            }
        }

        /// <summary>
        /// Command to navigate to OrdersPage
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task NavigateToOrdersPage()
        {
            // Navigate to ProfilePage
            try
            {
                var tabv = _serviceProvider.GetRequiredService<OrdersViewModel>();
                await Application.Current.MainPage.Navigation.PushAsync(new OrdersPage(tabv));
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-NavigateToOrdersPage Error", ex);
            }
        }

        /// <summary>
        /// Command to navigate to ManageMenuItemPage
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task NavigateToSettings()
        {
            // Navigate to SettingsPage
            try
            {
                var settingVM = _serviceProvider.GetRequiredService<SettingsViewModel>();
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage(settingVM));
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-NavigateToSettings Error", ex);
            }
        }

        /// <summary>
        /// Command to logout the user
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task LogoutFromApp()
        {
            // Navigate to SettingsPage
            try
            {
                var tableViewModel = _serviceProvider.GetRequiredService<TableViewModel>();
                await tableViewModel.LogoutCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-LogoutFromApp Error", ex);
            }
        }

        /// <summary>
        /// Command to open the support popup
        /// </summary>
        /// <returns>Return a Task Object</returns>
        [RelayCommand]
        private async Task SupportCommand()
        {
            try
            {
                var setting = _serviceProvider.GetRequiredService<Setting>();
                var helpPopup = new HelpPopup(setting);
                await Shell.Current.ShowPopupAsync(helpPopup);
            }
            catch (Exception ex)
            {
                _logger.LogError("ShellVM-SupportCommand Error", ex);
            }
        }
    }
}
