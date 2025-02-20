using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Sales Report operations
    /// </summary>
    public partial class SalesReportViewModel : ObservableObject
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
        /// To add order types in picker
        /// </summary>
        public ObservableCollection<ValueForPicker> OrderTypes { get; set; } = new();

        /// <summary>
        /// Selected type for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedType;

        /// <summary>
        /// ObservableCollection for KotItems
        /// </summary>
        public ObservableCollection<OrderPayment> SalesReportData { get; set; } = new();

        /// <summary>
        /// To know the total amount spent on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalSpent;

        /// <summary>
        /// To know the total amount spent in cash on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalCash;

        /// <summary>
        /// To know the total amount spent online on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalOnline;

        /// <summary>
        /// To know the total amount spent by bank or card on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalBank;

        /// <summary>
        /// To know the total pickup orders on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalPickup;

        /// <summary>
        /// To know the total dine in orders on the select date
        /// </summary>
        [ObservableProperty]
        private decimal _totalDine;

        /// <summary>
        /// Constructor for the OrdersViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public SalesReportViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
                SelectedDate = DateTime.Now;

                SalesReportData.Clear();

                SelectedType = OrderTypes[0];
                return;
            }

            _isInitialized = true;
            IsLoading = true;

            foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<OrderTypes>())
            {
                OrderTypes.Add(desc);
            }

            SelectedType = OrderTypes[0];

            await MakeSalesReport();

            IsLoading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeSalesReport()
        {
            SalesReportData.Clear();
            TotalSpent = TotalCash = TotalOnline = TotalBank = TotalDine = TotalPickup = 0;
            var orderEntries = await _databaseService.OrderPaymentOperations.GetFilteredOrderPaymentsAsync(SelectedDate, SelectedType.Key);

            if (orderEntries.Length > 0)
            {
                var orderItem = orderEntries.ToList();

                foreach (var order in orderItem)
                {
                    TotalSpent += order.Total;

                    if (order.OrderType == Data.OrderTypes.Pickup)
                        TotalPickup += 1;
                    else
                        TotalDine += 1;

                    switch (order.PaymentMode)
                    {
                        case PaymentModes.Cash:
                            TotalCash += order.Total;
                            break;
                        case PaymentModes.Online:
                            TotalOnline += order.Total;
                            break;
                        case PaymentModes.Card:
                            TotalBank += order.Total;
                            break;
                        case PaymentModes.Part:
                            TotalBank += order.PartPaidInCard;
                            TotalCash += order.PartPaidInCash;
                            TotalOnline += order.PartPaidInOnline;
                            break;
                    }
                    SalesReportData.Add(order);
                }
            }
        }

        /// <summary>
        /// To generate sales report with given paramters
        /// </summary>
        [RelayCommand]
        private async void Search()
        {
            if (SelectedType == null)
            {
                await Shell.Current.DisplayAlert("Search Error", "Select a order type", "Ok");
                return;
            }

            await MakeSalesReport();
        }
    }
}
