using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        public ObservableCollection<ValueForPicker> Items { get; set; } = new();

        /// <summary>
        /// ObservableCollection for KotItems
        /// </summary>
        public ObservableCollection<KOTItem> KOTItems { get; set; } = new();

        /// <summary>
        /// Selected type for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedItem;

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
        public ItemReportViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            var defaultOrderType = new ValueForPicker { Key = 0, Value = "All" };
            if (Items.Where(o => o.Key == 0) != null)
                SelectedItem = defaultOrderType;
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
                if (Items.Where(o => o.Key == 0) != null)
                    SelectedItem = defaultOrderType;

                KOTItems.Clear();
                TotalItems = 0;
                TotalQuantity = 0;
                TotalAmount = 0;

                return;
            }

            _isInitialized = true;
            IsLoading = true;

            var items = await _databaseService.GetMenuItemBySearch();

            Items.Add(defaultOrderType);
            foreach (var item in items)
            {
                Items.Add(new ValueForPicker
                {
                    Key = item.Id,
                    Value = item.Name
                });
            }

            await MakeItemReport();

            IsLoading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeItemReport()
        {
            KOTItems.Clear();
            var kotItems = await _databaseService.GetAllKotItemsAsync(SelectedDate, SelectedItem.Key);

            var groupedItems = kotItems.GroupBy(o => o.ItemId).ToDictionary(group => group.Key, group => group.ToArray());

            foreach (var kotItem in groupedItems)
            {
                KOTItems.Add(new KOTItem
                {
                    ItemId = kotItem.Key,
                    Name = kotItem.Value.First().Name,
                    Price = kotItem.Value.First().Price,
                    Quantity = kotItem.Value.Sum(o => o.Quantity)
                });
            }

            TotalItems = KOTItems.Count;
            TotalQuantity = KOTItems.Sum(o => o.Quantity);
            TotalAmount = KOTItems.Sum(o => o.Amount);
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

            await MakeItemReport();
        }
    }
}
