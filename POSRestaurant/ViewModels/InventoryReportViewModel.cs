using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Inventory Report operations
    /// </summary>
    public partial class InventoryReportViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

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
        public ObservableCollection<ValueForPicker> ItemTypes { get; set; } = new();

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
        private ValueForPicker _selectedItem;

        /// <summary>
        /// Selected payer for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedPayer;

        /// <summary>
        /// Constructor for the OrdersViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryReportViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };
            if (ItemTypes.Where(o => o.Key == 0) != null)
                SelectedItem = defaultOrderType;

            if (Payers.Where(o => o.Key == 0) != null)
                SelectedPayer = defaultOrderType;
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            //Reset page
            var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };
            if (_isInitialized)
            {
                SelectedDate= DateTime.Now;
                if (ItemTypes.Where(o => o.Key == 0) != null)
                    SelectedItem = defaultOrderType;

                if (Payers.Where(o => o.Key == 0) != null)
                    SelectedPayer = defaultOrderType;

                InventoryReportData.Clear();

                return;
            }

            _isInitialized = true;
            IsLoading = true;

            foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<ExpenseItemTypes>())
            {
                ItemTypes.Add(desc);
            }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeInventoryReport()
        {
            InventoryReportData.Clear();
            var inventoryEntries = await _databaseService.InventoryOperations.GetInventoryItemsAsync(SelectedDate, SelectedItem.Key, SelectedPayer.Key);

            if (inventoryEntries.Length > 0)
            {
                var inventoryItems = inventoryEntries.Select(InventoryReportModel.FromEntity)
                                    .ToList();

                foreach (var inventory in inventoryItems)
                {
                    InventoryReportData.Add(inventory);
                }
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
    }
}
