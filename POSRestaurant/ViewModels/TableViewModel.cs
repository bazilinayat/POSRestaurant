using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Controls;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Pages;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;
using System.Collections.ObjectModel;
using Windows.Devices.PointOfService;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel For Table Page
    /// </summary>
    public partial class TableViewModel : ObservableObject, 
        IRecipient<TableChangedMessage>, IRecipient<StaffChangedMessage>, 
        IRecipient<TableStateChangedMessage>, IRecipient<OrderChangedMessage>
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
        private readonly Setting _settingService;

        /// <summary>
        /// DIed PickupViewModel
        /// </summary>
        private readonly PickupViewModel _pickupViewModel;

        /// <summary>
        /// DIed BillingService
        /// </summary>
        private readonly BillingService _billingService;

        private readonly WindowsNotificationService _notificationService;

        /// <summary>
        /// List of waiters to be assigned to the order
        /// </summary>
        [ObservableProperty]
        public StaffModel[] _cashiers;

        /// <summary>
        /// To manage the selected waiter for the order
        /// </summary>
        [ObservableProperty]
        private StaffModel _selectedCashier;

        /// <summary>
        /// To have a list of running pickup orders
        /// </summary>
        public ObservableCollection<OrderModel> RunningPickupOrders { get; set; } = new();

        #region Login Fields

        /// <summary>
        /// DI for IAuthService
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// To accept the username
        /// </summary>
        [ObservableProperty]
        private string _username;

        /// <summary>
        /// To accept the password
        /// </summary>
        [ObservableProperty]
        private string _password;

        /// <summary>
        /// To know if the application is busy
        /// </summary>
        [ObservableProperty]
        private bool _isBusy;

        /// <summary>
        /// To display the error message
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;

        /// <summary>
        /// To know if login has error
        /// </summary>
        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// To know if the user is logged in or not
        /// </summary>
        [ObservableProperty]
        public bool _loggedIn;

        /// <summary>
        /// To know if the logged in user is admin
        /// </summary>
        [ObservableProperty]
        private bool _isAdminLogin;

        /// <summary>
        /// Method to handle successful command execution
        /// </summary>
        private void OnCommandExecutedSuccessfully()
        {
            // This event can be subscribed to by the view
            CommandCompletedSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that the view can subscribe to
        /// </summary>
        public event EventHandler CommandCompletedSuccessfully;

        /// <summary>
        /// Method to handle successful command execution
        /// </summary>
        private void OnLogoutExecutedSuccessfully()
        {
            // This event can be subscribed to by the view
            LogoutCompletedSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that the view can subscribe to
        /// </summary>
        public event EventHandler LogoutCompletedSuccessfully;

        #endregion

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
            OrdersViewModel ordersViewModel, Setting settingService,
            PickupViewModel pickupViewModel, BillingService billingService,
            IAuthService authService, WindowsNotificationService notificationService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _homeViewModel = homeViewModel;
            _settingService = settingService;
            _pickupViewModel = pickupViewModel;
            _billingService = billingService;
            _authService = authService;
            _notificationService = notificationService;

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<TableChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<StaffChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<TableStateChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<OrderChangedMessage>(this);
        }

        /// <summary>
        /// Command for logging in
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Login()
        {
            (Application.Current.MainPage as Shell)?.HideAllTabs();
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;
                HasError = false;

                var result = await _authService.LoginAsync(Username, Password);

                if (result)
                {
                    var info = await _databaseService.SettingsOperation.GetRestaurantInfo();
                    if (info == null)
                    {
                        if (_authService.GetCurrentUser().Username == "Admin")
                        {
                            await GetRestaurantInfo();
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Error", "Admin Login Required to set Restaurant Details", "OK");
                            LoggedIn = false;
                            return;
                        }
                    }
                    if (_authService.GetCurrentUser().Username == "Admin")
                    {
                        IsAdminLogin = true;
                        (App.Current.MainPage as AppShell)?.UpdateSetting(true);
                    }
                    else
                    {
                        IsAdminLogin = false;
                        (App.Current.MainPage as AppShell)?.UpdateSetting(false);
                    }
                    LoggedIn = true;
                    (Application.Current.MainPage as Shell)?.ConfigureTabVisibility(_authService);
                    OnCommandExecutedSuccessfully();
                    await Task.Delay(10);
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login error: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command for logging out
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Logout()
        {
            LoggedIn = IsAdminLogin = false;
            (Application.Current.MainPage as Shell)?.HideAllTabs();
            OnLogoutExecutedSuccessfully();
            await _authService.LogoutAsync();
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
                if (LoggedIn)
                {
                    (Application.Current.MainPage as Shell)?.HideAllTabs();
                    (Application.Current.MainPage as Shell)?.ConfigureTabVisibility(_authService);
                    OnCommandExecutedSuccessfully();
                }

                if (_isInitialized)
                    return;

                _isInitialized = true;

                IsLoading = true;

                await GetTablesAsync();

                await GetTableStateAsync();

                await GetRunningPickups();

                await LoadCashiers();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "There was an error loading the screen", "OK");
                throw;
            }
        }

        /// <summary>
        /// To get the restaurant info from user
        /// </summary>
        /// <returns>Returns task</returns>
        private async Task GetRestaurantInfo()
        {
            var info = await _databaseService.SettingsOperation.GetRestaurantInfo();
            if (info == null)
            {
                var settingsViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
                var setInfo = new InitialPopup(settingsViewModel);
                await Shell.Current.ShowPopupAsync(setInfo);
            }
        }

        /// <summary>
        /// To call the database and load the list of waiters
        /// </summary>
        /// <returns>Returns a task object</returns>
        private async Task LoadCashiers()
        {
            try
            {
                Cashiers = (await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.Cashier))
                                    .Select(StaffModel.FromEntity)
                                    .ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-LoadCashiers Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Cashiers", "OK");
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
        /// To get all the pickup orders which are running
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        public async ValueTask GetTableStateAsync()
        {
            try
            {
                var tables = (await _databaseService.TableOperations.GetTableState())
                                    .Select(TableState.FromEntity)
                                    .ToArray();

                if (tables.Count() > 0)
                {
                    foreach (var table in tables)
                    {
                        var staff = await _databaseService.StaffOperaiotns.GetStaffBasedOnId(table.WaiterId);
                        if (staff != null)
                            table.Waiter = StaffModel.FromEntity(staff);

                        Receive(new TableChangedMessage(table));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-GetTablesAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// To get the latest table state
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        public async ValueTask GetRunningPickups()
        {
            try
            {
                var orders = (await _databaseService.GetRunningPickupOrders())
                                    .Select(OrderModel.FromEntity)
                                    .ToArray();

                if (orders.Count() > 0)
                {
                    foreach(var order in orders)
                    {
                        Receive(new OrderChangedMessage(order));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-GetRunningPickups Error", ex);
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
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Menu", "OK");
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
                if (SelectedCashier == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Select Cashier before Proceeding", "OK");
                    return;
                }

                await Application.Current.MainPage.Navigation.PushAsync(new PickupPage(_pickupViewModel, SelectedCashier));
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-MakePickupOrder Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Pickup Menu", "OK");
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
                await Shell.Current.DisplayAlert("Fault", "Error in Adding new Table", "OK");
            }
        }

        /// <summary>
        /// To receive the table change message coming from different places
        /// </summary>
        /// <param name="tableChangedMessage">Table details changed</param>
        public void Receive(TableChangedMessage tableChangedMessage)
        {
            var tableModel = tableChangedMessage.Value;

            if (tableModel.Status == TableOrderStatus.NoOrder)
            {
                for (int i = 0; i < Tables.Length; i++)
                {
                    if (Tables[i].Id == tableModel.Id)
                    {
                        Tables[i].Status = TableOrderStatus.NoOrder;
                        Tables[i].RunningOrderId = 0;
                        Tables[i].Waiter = null;
                        Tables[i].NumberOfPeople = 0;
                        Tables[i].OrderTotal = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Tables.Length; i++)
                {
                    if (Tables[i].Id == tableModel.Id)
                    {
                        Tables[i] = tableModel;
                    }
                }
            }

            TransformTables(Tables);
        }

        /// <summary>
        /// To receive the table state change message coming from different places
        /// To maintain the state
        /// </summary>
        /// <param name="tableStateChangedMessage">Table details changed</param>
        public async void Receive(TableStateChangedMessage tableStateChangedMessage)
        {
            var tableModel = tableStateChangedMessage.Value;
            switch (tableModel.Status)
            {
                case TableOrderStatus.Running: // To update the endtime and save
                case TableOrderStatus.Confirmed: // To update the endtime and save
                    tableModel.EndTime = DateTime.Now;
                    await _databaseService.TableOperations.SaveTableStateAsync(tableModel);
                    break;

                case TableOrderStatus.NoOrder:
                    await _databaseService.TableOperations.DeleteTableStateAsync(tableModel);
                    break;
            }

            TransformTables(Tables);
        }

        /// <summary>
        /// Refresh staff details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(StaffChangedMessage message)
        {
            await LoadCashiers();
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
                        await PrintFinalBill(tableModel);
                        await _databaseService.TableOperations.SaveTableStateAsync(tableModel);
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
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Actions", "OK");
            }
        }

        /// <summary>
        /// To print the final bill of the selected table
        /// </summary>
        /// <param name="tableModel">TableModel for selected table</param>
        /// <returns>Returns Task</returns>
        private async Task PrintFinalBill(TableModel tableModel)
        {
            IsLoading = true;
            await Task.Delay(10);
            try
            {
                if (SelectedCashier == null)
                {
                    await Shell.Current.DisplayAlert("Printing Error", "Assign a cashier to the order.", "Ok");
                    IsLoading = false;
                    return;
                }

                tableModel.Cashier = SelectedCashier;
                tableModel.CashierId = SelectedCashier.Id;

                await _billingService.PrintBill(tableModel);

                tableModel.Status = Data.TableOrderStatus.Printed;
                tableModel.OrderTotal = _billingService.OrderModel.GrandTotal;

                var order = await _databaseService.GetOrderById(tableModel.RunningOrderId);
                order.OrderStatus = TableOrderStatus.Printed;
                await _databaseService.UpdateOrder(order);

                Receive(new TableChangedMessage(tableModel));
                Receive(new TableStateChangedMessage(tableModel));

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-PrintFinalBill Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Printing the Bill", "OK");
                IsLoading = false;
            }
        }

        /// <summary>
        /// To know that there are some changes in order details
        /// </summary>
        /// <param name="orderChangedMessage">Order Details are changed</param>
        public void Receive(OrderChangedMessage orderChangedMessage)
        {
            var receivedOrder = orderChangedMessage.Value;
            if (receivedOrder != null)
            {
                if (receivedOrder.OrderType != OrderTypes.Pickup)
                    return;

                if (receivedOrder.OrderStatus == TableOrderStatus.Paid)
                {
                    var orderToRemove = RunningPickupOrders.Where(o => o.Id == receivedOrder.Id).FirstOrDefault();
                    if (orderToRemove != null)
                        RunningPickupOrders.Remove(orderToRemove);
                }
                else
                {
                    RunningPickupOrders.Add(receivedOrder);
                }
            }
        }

        /// <summary>
        /// When the pickup order is clicked from the list
        /// </summary>
        /// <param name="orderModel">OrderModel for selected pickup order</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task PickupComplete(OrderModel orderModel)
        {
            try
            {
                var orderCompleteVM = _serviceProvider.GetRequiredService<OrderCompleteViewModel>();
                var completeOrderPopup = new OrderCompletePopup(orderCompleteVM, null, orderModel);
                await Shell.Current.ShowPopupAsync(completeOrderPopup);
            }
            catch (Exception ex)
            {
                _logger.LogError("TableVM-PickupComplete Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Actions", "OK");
            }
        }
    }
}
