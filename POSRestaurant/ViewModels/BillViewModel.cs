using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using POSRestaurant.ChangedMessages;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Utility;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Bill Page
    /// </summary>
    public partial class BillViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;
        
        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly TaxService _taxService;

        /// <summary>
        /// To use for order details
        /// </summary>
        public TableModel TableModel { get; set; }

        /// <summary>
        /// To store the order details
        /// </summary>
        [ObservableProperty]
        private OrderModel _orderModel;

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public List<KOTModel> OrderKOTs { get; set; } = new();

        /// <summary>
        /// Comma separated order kot ids for this order
        /// </summary>
        [ObservableProperty]
        private string _orderKOTIds;

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public ObservableCollection<KOTItemBillModel> OrderKOTItems { get; set; } = new();

        /// <summary>
        /// Total quantity of items ordered
        /// </summary>
        [ObservableProperty]
        private int _totalQuantity;

        /// <summary>
        /// Total price of all the items
        /// </summary>
        [ObservableProperty]
        private decimal _subTotal;

        /// <summary>
        /// Round off of the order to get grand total
        /// </summary>
        [ObservableProperty]
        private decimal _roundOff;

        /// <summary>
        /// Grand total of the order, to be paid by the customer
        /// </summary>
        [ObservableProperty]
        private decimal _grandTotal;

        /// <summary>
        /// CGST Amount of the order
        /// </summary>
        [ObservableProperty]
        private decimal _cGST;

        /// <summary>
        /// SGST Amount of the order
        /// </summary>
        [ObservableProperty]
        private decimal _sGST;

        /// <summary>
        /// CGST Amount of the order
        /// </summary>
        [ObservableProperty]
        private decimal _cGSTAmount;

        /// <summary>
        /// SGST Amount of the order
        /// </summary>
        [ObservableProperty]
        private decimal _sGSTAmount;

        /// <summary>
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public BillViewModel(DatabaseService databaseService, OrdersViewModel ordersViewModel, SettingService settingService, TaxService taxService)
        {
            _databaseService = databaseService;
            _settingService = settingService;
            _taxService = taxService;
        }

        /*
        * Take RunningOrderId from TableModel
        * Get Order Details
        * Get KOT Details
        * Get KOTItems
        * From these above, we will get -
        *   Order Number
        *   Order DateTime
        *   Order Type
        *   Token No - {KOT Ids}
        *   Items
        * 
        * Available in TableModel -
        *   Number Of People
        *   Waiter Details
        *   
        * Tax Details -
        *   To be taken from Tax Service
        *   
        * To Calculate -
        *   Total Quantity - Total Number of Items
        *   Total Price - Sum of Amount of all KOTs
        *   Round-Off
        *   Grand Total
        *   
        */

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            IsLoading = true;

            OrderKOTs.Clear();
            OrderKOTItems.Clear();
            OrderKOTIds = "";

            await GetOrderDetailsAsync();

            IsLoading = false;
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task GetOrderDetailsAsync()
        {
            // Get Order for Basic Details
            var order = await _databaseService.GetOrderById(TableModel.RunningOrderId);

            OrderModel = new OrderModel
            {
                Id = order.Id,
                TableId = order.TableId,
                OrderDate = order.OrderDate,
                OrderType = order.OrderType,
                TotalItemCount = order.TotalItemCount,
                TotalPrice = order.TotalPrice,
                PaymentMode = order.PaymentMode,
                OrderStatus = order.OrderStatus,
            };

            // Get Order KOTs for More Details
            OrderKOTs = (await _databaseService.GetOrderKotsAsync(OrderModel.Id))
                            .Select(KOTModel.FromEntity)
                            .ToList();

            OrderKOTIds = string.Join(',', OrderKOTs.Select(o => o.Id).ToArray());

            /*
             * Get Order KOT Items
             * Group them together
             * Calculcate totals
             */
            var kotItems = new List<KOTItemModel>();

            foreach (var kot in OrderKOTs)
            {
                var items = (await _databaseService.GetKotItemsAsync(kot.Id))
                            .Select(KOTItemModel.FromEntity)
                            .ToList();

                kotItems.AddRange(items);
            }

            // Group items together
            var dict = kotItems.GroupBy(o => o.ItemId).ToDictionary(g => g.Key, g => g.Select(o => o));

            foreach(var groupedItems in dict)
            {
                OrderKOTItems.Add(new KOTItemBillModel
                {
                    ItemId = groupedItems.Key,
                    Name = groupedItems.Value.First().Name,
                    Quantity = groupedItems.Value.Sum(o => o.Quantity),
                    Price = groupedItems.Value.First().Price,
                });
            }

            // Calculate totals
            TotalQuantity = OrderKOTItems.Sum(o => o.Quantity);
            SubTotal = OrderKOTItems.Sum(o => o.Amount);

            CGST = _taxService.IndianTaxService.CGST;
            SGST = _taxService.IndianTaxService.SGST;

            CGSTAmount = _taxService.IndianTaxService.CalculateCGST(SubTotal);
            SGSTAmount = _taxService.IndianTaxService.CalculateSGST(SubTotal);

            var total = SubTotal + CGSTAmount + SGSTAmount;
            GrandTotal = Math.Floor(total);

            RoundOff = GrandTotal - total;
        }

        /// <summary>
        /// Command to call when we want to print the receipt
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task PrintReceiptAsync()
        {
            // TODO: Printing

            await Shell.Current.DisplayAlert("Printing", "Printing Taking Place", "OK");

            TableModel.Status = Data.TableOrderStatus.Printed;
            TableModel.OrderTotal = GrandTotal;

            WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));
        }
    }
}
