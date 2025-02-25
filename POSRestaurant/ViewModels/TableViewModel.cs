using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Controls;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Pages;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel For Table Page
    /// </summary>
    public partial class TableViewModel : ObservableObject, IRecipient<TableChangedMessage>
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// To get and display all the categories
        /// Made observable to use everywhere
        /// </summary>
        [ObservableProperty]
        private TableModel[] _tables = [];

        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// DIed OrdersViewModel
        /// </summary>
        private readonly OrdersViewModel _ordersViewModel;
        
        /// <summary>
        /// DIed HomeViewModel
        /// </summary>
        private readonly HomeViewModel _homeViewModel;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// DIed PickupViewModel
        /// </summary>
        private readonly PickupViewModel _pickupViewModel;

        /// <summary>
        /// Constructor for the TablesViewModel
        /// </summary>
        /// <param name="serviceProvider">DI for IServiceProvider</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="homeViewModel">DI for HomeViewModel</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public TableViewModel(IServiceProvider serviceProvider, LogService logger, 
            DatabaseService databaseService, HomeViewModel homeViewModel, 
            OrdersViewModel ordersViewModel, SettingService settingService,
            PickupViewModel pickupViewModel)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _homeViewModel = homeViewModel;
            _settingService = settingService;
            _pickupViewModel = pickupViewModel;

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<TableChangedMessage>(this);
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            try
            {
                var info = await _databaseService.SettingsOperation.GetRestaurantInfo();
                if (info == null)
                {
                    var settingsViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
                    var setInfo = new InitialPopup(settingsViewModel);
                    await Shell.Current.ShowPopupAsync(setInfo);
                }

                if (_isInitialized)
                    return;

                _isInitialized = true;

                IsLoading = true;

                await GetTablesAsync();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-InitializeAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// To get or update the table details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        public async ValueTask GetTablesAsync()
        {
            try
            {
                var tables = (await _databaseService.TableOperations.GetTablesAsync())
                                    .Select(TableModel.FromEntity)
                                    .ToArray();

                TransformTables(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-GetTablesAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// To be called each time we get the tables from database
        /// This will transform and modify the data in table list, to be used for the UI
        /// </summary>
        /// <param name="tables">List of TableModel</param>
        private void TransformTables(TableModel[] tables)
        {
            try
            {
                IsLoading = true;

                foreach (var table in tables)
                {
                    switch (table.Status)
                    {
                        case TableOrderStatus.NoOrder:
                            // Used to reset everything
                            table.BorderColour = Colors.Brown;
                            table.ActionButtonImageIcon = "check_circle_regular_24.png";
                            table.ActionButtonEnabled = false;
                            break;
                        case TableOrderStatus.Running:
                            table.BorderColour = Colors.Yellow;
                            table.ActionButtonImageIcon = "eye.png";
                            table.ActionButtonEnabled = true;
                            break;
                        case TableOrderStatus.Confirmed:
                            table.BorderColour = Colors.Orange;
                            table.ActionButtonImageIcon = "invoice.png";
                            table.ActionButtonEnabled = true;
                            break;
                        case TableOrderStatus.Printed:
                            table.BorderColour = Colors.Green;
                            table.ActionButtonImageIcon = "diskette.png";
                            table.ActionButtonEnabled = true;
                            break;
                        case TableOrderStatus.Paid:
                            table.BorderColour = Colors.Green;
                            break;
                    }
                }

                Tables = [.. tables];

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-TransformTables Error", ex);
                throw;
            }
        }

        /// <summary>
        /// When a table is selected, show menu popup
        /// </summary>
        /// <param name="tableModel">TableModel for selected table</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task TableSelected(TableModel tableModel)
        {
            try
            {
                if (tableModel.Status == TableOrderStatus.Confirmed || tableModel.Status == TableOrderStatus.Printed)
                    return;

                await Application.Current.MainPage.Navigation.PushAsync(new MainPage(_homeViewModel, tableModel));
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-TableSelected Error", ex);
                throw;
            }
        }

        /// <summary>
        /// When pickup button is clicked, to go to pickup page for placing order
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task MakePickupOrder()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PickupPage(_pickupViewModel));
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-MakePickupOrder Error", ex);
                throw;
            }
        }

        /// <summary>
        /// When add new table button is clicked, confirm and add a new table
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task AddNewTable()
        {
            try
            {
                if (await Shell.Current.DisplayAlert("Add Table", $"Do you really want to add a new table?", "Yes", "No"))
                {
                    var errorMessage = await _databaseService.TableOperations.AddNewTableAsync();

                    if (errorMessage != null)
                    {
                        await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                        return;
                    }

                    await GetTablesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-AddNewTable Error", ex);
                throw;
            }
        }

        /// <summary>
        /// To receive the table change message coming from different places
        /// </summary>
        /// <param name="tableChangedMessage">Table details changed</param>
        public void Receive(TableChangedMessage tableChangedMessage)
        {
            var tableModel = tableChangedMessage.Value;
            for(int i = 0; i < Tables.Length; i++)
            {
                if (Tables[i].Id == tableModel.Id)
                {
                    Tables[i] = tableModel;
                }
            }

            TransformTables(Tables);
        }

        /// <summary>
        /// When the action button for a table is clicked
        /// Action will depend on RunningOrderId and Status
        /// </summary>
        /// <param name="tableModel">TableModel for selected table</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task TableActionButton(TableModel tableModel)
        {
            try
            {
                switch (tableModel.Status)
                {
                    case TableOrderStatus.Running: // To view the whole order, with KOTs
                        var vovm = _serviceProvider.GetRequiredService<OrderViewViewModel>();
                        await Application.Current.MainPage.Navigation.PushAsync(new OrderViewPage(vovm, _ordersViewModel, tableModel));
                        break;
                    case TableOrderStatus.Confirmed:
                        var billvm = _serviceProvider.GetRequiredService<BillViewModel>();
                        await Application.Current.MainPage.Navigation.PushAsync(new BillPage(billvm, tableModel));
                        break;
                    case TableOrderStatus.Printed: // To show popup, for marking as paid
                        var orderCompleteVM = _serviceProvider.GetRequiredService<OrderCompleteViewModel>();
                        var completeOrderPopup = new OrderCompletePopup(orderCompleteVM, tableModel);
                        await Shell.Current.ShowPopupAsync(completeOrderPopup);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-TableActionButton Error", ex);
                throw;
            }
        }
    }
}
