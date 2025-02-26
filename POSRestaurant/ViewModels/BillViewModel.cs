using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;
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
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

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
        /// DIed ReceiptService
        /// </summary>
        private readonly ReceiptService _receiptService;

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
        /// To display the discount amount on UI
        /// </summary>
        [ObservableProperty]
        private decimal _discountAmount;

        /// <summary>
        /// To decide to show or not the discount variables
        /// </summary>
        [ObservableProperty]
        private bool _showDiscountVariables;

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
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public BillViewModel(DatabaseService databaseService, LogService logger, OrdersViewModel ordersViewModel, 
            SettingService settingService, ReceiptService receiptService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _settingService = settingService;
            _receiptService = receiptService;
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
            try
            {
                IsLoading = true;

                OrderKOTs.Clear();
                OrderKOTItems.Clear();
                OrderKOTIds = "";
                ShowDiscountVariables = false;

                await GetOrderDetailsAsync();

                await LoadCashiers();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("BillVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Bill Screen", "OK");
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
                _logger.LogError("BillVM-LoadCashiers Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Cashiers", "OK");
            }
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task GetOrderDetailsAsync()
        {
            IsLoading = true;
            try
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
                    TotalAmount = order.TotalAmount,
                    PaymentMode = order.PaymentMode,
                    OrderStatus = order.OrderStatus,

                    IsDiscountGiven = order.IsDiscountGiven,
                    IsFixedBased = order.IsFixedBased,
                    IsPercentageBased = order.IsPercentageBased,
                    DiscountFixed = order.DiscountFixed,
                    DiscountPercentage = order.DiscountPercentage,
                    TotalAmountAfterDiscount = order.TotalAmountAfterDiscount,

                    UsingGST = order.UsingGST,
                    CGST = order.CGST,
                    SGST = order.SGST,
                    CGSTAmount = order.CGSTAmount,
                    SGSTAmount = order.SGSTAmount,

                    RoundOff = order.RoundOff,
                    GrandTotal = order.GrandTotal,
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

                foreach (var groupedItems in dict)
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

                if (OrderModel.IsDiscountGiven)
                {
                    ShowDiscountVariables = true;
                    if (OrderModel.IsFixedBased)
                    {
                        DiscountAmount = OrderModel.DiscountFixed;
                    }
                    else if (OrderModel.IsPercentageBased)
                    {
                        DiscountAmount = OrderModel.TotalAmount * OrderModel.DiscountPercentage / 100;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("BillVM-GetOrderDetailsAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Bill Details", "OK");
            }
            IsLoading = false;
        }

        /// <summary>
        /// Command to call when we want to print the receipt
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task PrintReceiptAsync()
        {
            IsLoading = true;
            await Task.Delay(10); // Give UI time to update
            try
            {
                if (SelectedCashier == null)
                {
                    await Shell.Current.DisplayAlert("Printing Error", "Assign a cashier to the order.", "Ok");
                    IsLoading = false;
                    return;
                }

                var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

                var billModel = new BillModel
                {
                    RestrauntName = restaurantInfo.Name,
                    Address = restaurantInfo.Address,
                    GSTIn = restaurantInfo.GSTIN,
                    CustomerName = "Customer Name",

                    OrderType = OrderModel.OrderType,

                    TimeStamp = OrderModel.OrderDate,
                    TableNo = TableModel.TableNo,
                    Cashier = SelectedCashier.Name,
                    BillNo = OrderModel.Id.ToString(),
                    TokenNos = OrderKOTIds,
                    WaiterAssigned = TableModel.Waiter.Name,

                    Items = OrderKOTItems.ToList(),

                    TotalQty = OrderModel.TotalItemCount,
                    SubTotal = OrderModel.TotalAmount,

                    IsDiscountGiven = OrderModel.IsDiscountGiven,
                    IsFixedBased = OrderModel.IsFixedBased,
                    IsPercentageBased = OrderModel.IsPercentageBased,
                    DiscountFixed = OrderModel.DiscountFixed,
                    DiscountPercentage = OrderModel.DiscountPercentage,
                    SubTotalAfterDiscount = OrderModel.TotalAmountAfterDiscount,

                    UsginGST = OrderModel.UsingGST,
                    CGST = OrderModel.CGST,
                    SGST = OrderModel.SGST,
                    CGSTAmount = OrderModel.CGSTAmount,
                    SGSTAmount = OrderModel.SGSTAmount,
                    RoundOff = OrderModel.RoundOff,
                    GrandTotal = OrderModel.GrandTotal,

                    FassaiNo = restaurantInfo.FSSAI,
                    QRCode = "Data"
                };

                var pdfData = await _receiptService.GenerateReceipt(billModel);
                await _receiptService.PrintReceipt(pdfData);

                TableModel.Status = Data.TableOrderStatus.Printed;
                TableModel.OrderTotal = OrderModel.GrandTotal;

                WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("BillVM-PrintReceiptAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Printing the Bill", "OK");
                IsLoading = false;
            }
            finally
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
