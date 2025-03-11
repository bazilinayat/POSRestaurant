using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Item Report operations
    /// </summary>
    public partial class ItemReportViewModel : ObservableObject
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
        /// DIed variable for MenuService
        /// </summary>
        private readonly MenuService _menuService;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// Selected date from the Order page
        /// </summary>
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        /// <summary>
        /// To add order types in picker
        /// </summary>
        public ObservableCollection<ValueForPicker> Categories { get; set; } = new();

        /// <summary>
        /// ObservableCollection for KotItems
        /// </summary>
        public ObservableCollection<ItemReportModel> ItemReportData { get; set; } = new();

        /// <summary>
        /// Item data to display
        /// </summary>
        [ObservableProperty]
        private Dictionary<string, List<KOTItem>> _itemData = new();

        /// <summary>
        /// Selected type for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedCategory;

        /// <summary>
        /// To show the total number of items
        /// </summary>
        [ObservableProperty]
        private long _totalItems;

        /// <summary>
        /// To show the total quantity of items
        /// </summary>
        [ObservableProperty]
        private long _totalQuantity;

        /// <summary>
        /// To show the toal amount of items
        /// </summary>
        [ObservableProperty]
        private decimal _totalAmount;

        /// <summary>
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Constructor for the OrdersViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public ItemReportViewModel(LogService logger, DatabaseService databaseService, MenuService menuService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _menuService = menuService;

            var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };
            if (Categories.Where(o => o.Key == 0) != null)
                SelectedCategory = defaultOrderType;
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
                    if (Categories.Where(o => o.Key == 0) != null)
                        SelectedCategory = defaultOrderType;

                    ItemReportData.Clear();
                    TotalItems = 0;
                    TotalQuantity = 0;
                    TotalAmount = 0;

                    return;
                }

                _isInitialized = true;
                IsLoading = true;

                var categories = await _menuService.GetMenuCategories();

                Categories.Add(defaultOrderType);
                foreach (var category in categories)
                {
                    Categories.Add(new ValueForPicker
                    {
                        Key = category.Id,
                        Value = category.Name
                    });
                }

                await MakeItemReport();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("ItemReportVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Item Report", "OK");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeItemReport()
        {
            try
            {
                ItemReportData.Clear();
                var kotItems = await _databaseService.GetAllKotItemsAsync(SelectedDate);

                var groupedItems = kotItems.GroupBy(o => o.ItemId).ToDictionary(group => group.Key, group => group.ToArray());

                foreach (var kotItem in groupedItems)
                {
                    var category = await _databaseService.MenuOperations.GetCategoryOfMenuItem(kotItem.Key);

                    if (SelectedCategory.Key != 0)
                    {
                        if (category.Id != SelectedCategory.Key)
                            continue;
                    }

                    var totalQuantity = kotItem.Value.Sum(o => o.Quantity);
                    var itemPrice = kotItem.Value.First().Price;

                    var kotItemsList = new List<KOTItem>
                    {
                        new KOTItem
                        {
                            ItemId = kotItem.Key,
                            Name = kotItem.Value.First().Name,
                            Price = itemPrice,
                            Quantity = totalQuantity,
                        }
                    };

                    var existingCategory = ItemReportData.FirstOrDefault(o => o.CategoryName == category.Name);
                    if (existingCategory != null)
                    {
                        existingCategory.KOTItems.AddRange(kotItemsList);
                    }
                    else
                    {
                        ItemReportData.Add(new ItemReportModel
                        {
                            CategoryName = category.Name,
                            KOTItems = kotItemsList
                        });
                    }
                }

                // Calculate totals
                TotalItems = ItemReportData.Sum(category => category.KOTItems.Count);
                TotalQuantity = ItemReportData.Sum(category => category.KOTItems.Sum(item => item.Quantity));
                TotalAmount = ItemReportData.Sum(category => category.KOTItems.Sum(item => item.Amount));
            }
            catch (Exception ex)
            {
                _logger.LogError("ItemReportVM-MakeItemReport Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Generating Item Report", "OK");
            }
        }

        /// <summary>
        /// To search the order with given paramters
        /// </summary>
        [RelayCommand]
        private async void Search()
        {
            if (SelectedCategory == null)
            {
                await Shell.Current.DisplayAlert("Search Error", "Select a item", "Ok");
                return;
            }

            await MakeItemReport();
        }
    }
}
