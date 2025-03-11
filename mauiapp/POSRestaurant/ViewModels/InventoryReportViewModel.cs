using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Inventory Report operations
    /// </summary>
    public partial class InventoryReportViewModel : ObservableObject, IRecipient<ExpenseTypeChangedMessage>
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
        /// Selected date from the Order page
        /// </summary>
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        /// <summary>
        /// To add expense types in picker
        /// </summary>
        public ObservableCollection<ExpenseTypeModel> ItemTypes { get; set; } = new();

        /// <summary>
        /// To add payers in picker
        /// </summary>
        public ObservableCollection<ValueForPicker> Payers { get; set; } = new();

        /// <summary>
        /// ObservableCollection for KotItems
        /// </summary>
        public ObservableCollection<InventoryReportModel> InventoryReportData { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty]
        private Dictionary<string, List<Inventory>> _itemData = new();

        /// <summary>
        /// Selected type for filter
        /// </summary>
        [ObservableProperty]
        private ExpenseTypeModel _selectedItem;

        /// <summary>
        /// Selected payer for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedPayer;

        /// <summary>
        /// To know the total amount spent on the select date
        /// </summary>
        [ObservableProperty]
        private double _totalSpent;

        /// <summary>
        /// To know the total amount spent in cash on the select date
        /// </summary>
        [ObservableProperty]
        private double _totalCash;

        /// <summary>
        /// To know the total amount spent online on the select date
        /// </summary>
        [ObservableProperty]
        private double _totalOnline;

        /// <summary>
        /// To know the total amount spent by bank or card on the select date
        /// </summary>
        [ObservableProperty]
        private double _totalBank;

        /// <summary>
        /// Constructor for the OrdersViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryReportViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;

            var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };

            if (Payers.Where(o => o.Key == 0) != null)
                SelectedPayer = defaultOrderType;

            WeakReferenceMessenger.Default.Register<ExpenseTypeChangedMessage>(this);
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
                //Reset page
                var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };
                if (_isInitialized)
                {
                    SelectedDate = DateTime.Now;

                    if (Payers.Where(o => o.Key == 0) != null)
                        SelectedPayer = defaultOrderType;

                    InventoryReportData.Clear();

                    return;
                }

                _isInitialized = true;
                IsLoading = true;

                var expenseItems = (await _databaseService.SettingsOperation.GetExpenseTypes()).ToList()
                                    .Select(ExpenseTypeModel.FromEntity)
                                    .ToList();
                ItemTypes.Add(new ExpenseTypeModel { Id = 0, Name = "All", IsSelected = false });
                foreach (var coowner in expenseItems)
                {
                    ItemTypes.Add(coowner);
                }
                ItemTypes[0].IsSelected = true;

                Payers.Add(defaultOrderType);
                var payers = await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.CoOwner);
                foreach (var payer in payers)
                {
                    Payers.Add(new ValueForPicker
                    {
                        Key = payer.Id,
                        Value = payer.Name,
                    });
                }

                await MakeInventoryReport();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryReportVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Inventory Report Screen", "OK");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeInventoryReport()
        {
            try
            {
                InventoryReportData.Clear();
                TotalSpent = TotalCash = TotalOnline = TotalBank = 0;
                var inventoryEntries = await _databaseService.InventoryOperations.GetInventoryItemsAsync(SelectedDate, SelectedItem == null ? 0 : SelectedItem.Id, SelectedPayer.Key);

                if (inventoryEntries.Length > 0)
                {
                    var inventoryItems = inventoryEntries.Select(InventoryReportModel.FromEntity)
                                        .ToList();

                    foreach (var inventory in inventoryItems)
                    {
                        TotalSpent += inventory.TotalPrice;
                        switch (inventory.PaymentMode)
                        {
                            case ExpensePaymentModes.Cash:
                                TotalCash += inventory.TotalPrice;
                                break;
                            case ExpensePaymentModes.Online:
                                TotalOnline += inventory.TotalPrice;
                                break;
                            case ExpensePaymentModes.Bank:
                                TotalBank += inventory.TotalPrice;
                                break;
                        }
                        InventoryReportData.Add(inventory);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryReportVM-MakeInventoryReport Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Generating Inventory Report", "OK");
            }
            
            //TotalItems = KOTItems.Count;
            //TotalQuantity = KOTItems.Sum(o => o.Quantity);
            //TotalAmount = KOTItems.Sum(o => o.Amount);
        }

        /// <summary>
        /// To search the order with given paramters
        /// </summary>
        [RelayCommand]
        private async void Search()
        {
            if (SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("Search Error", "Select a item", "Ok");
                return;
            }

            await MakeInventoryReport();
        }

        /// <summary>
        /// Refresh expense type details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(ExpenseTypeChangedMessage message)
        {
            try
            {
                ItemTypes.Clear();
                var expenseItems = (await _databaseService.SettingsOperation.GetExpenseTypes()).ToList()
                                    .Select(ExpenseTypeModel.FromEntity)
                                    .ToList();
                ItemTypes.Add(new ExpenseTypeModel { Id = 0, Name = "All", IsSelected = false });
                foreach (var coowner in expenseItems)
                {
                    ItemTypes.Add(coowner);
                }
                ItemTypes[0].IsSelected = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryReportVM-Receive ExpenseTypeChangedMessage Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error while Changing Expense Types", "OK");
            }
        }
    }
}
