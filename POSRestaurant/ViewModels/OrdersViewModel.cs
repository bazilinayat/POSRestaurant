using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Order operations
    /// </summary>
    public partial class OrdersViewModel : ObservableObject, IRecipient<OrderChangedMessage>
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
        /// ObservableCollection for orders
        /// </summary>
        public ObservableCollection<OrderModel> Orders { get; set; } = new();

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
        public ObservableCollection<ValueForPicker> OrderTypes { get; set; } = new();

        /// <summary>
        /// Selected type for filter
        /// </summary>
        [ObservableProperty]
        private ValueForPicker _selectedType;

        /// <summary>
        /// Current page of the table
        /// </summary>
        private int _currentPage = 1;

        /// <summary>
        /// Default page size for the table
        /// </summary>
        private const int PageSize = 5;

        /// <summary>
        /// Current page on the table of orders
        /// </summary>
        [ObservableProperty]
        private string _currentPageLabel = "Page: 1";

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        [ObservableProperty]
        private OrderItem[] _orderItems = [];

        /// <summary>
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly TaxService _taxService;

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
        /// To print order id on screen
        /// </summary>
        [ObservableProperty]
        private int _tableNo;

        /// <summary>
        /// To print the waiter name on screen
        /// </summary>
        [ObservableProperty]
        private string _waiterName;

        /// <summary>
        /// To print order id on screen
        /// </summary>
        [ObservableProperty]
        private OrderModel _orderToShow;

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
        /// To show the payment mode on the screen
        /// </summary>
        [ObservableProperty]
        private string _paymentMode;

        /// <summary>
        /// To show or not to show the order details
        /// Should not be shown until order is selected
        /// </summary>
        [ObservableProperty]
        private bool _orderDetailsVisible = false;

        /// <summary>
        /// To get the discount details of the order, if any
        /// </summary>
        [ObservableProperty]
        private Discount _discount;

        /// <summary>
        /// To display the discount amount on UI
        /// </summary>
        [ObservableProperty]
        private decimal _discountAmount;

        /// <summary>
        /// To display the subtotal after discount on UI
        /// </summary>
        [ObservableProperty]
        private decimal _subTotalAfterDiscount;

        /// <summary>
        /// To decide to show or not the discount variables
        /// </summary>
        [ObservableProperty]
        private bool _showDiscountVariables;

        /// <summary>
        /// To know if the payment is done in parts
        /// </summary>
        [ObservableProperty]
        private bool _isPartPayment;

        /// <summary>
        /// To know if the payment is done in parts
        /// </summary>
        [ObservableProperty]
        private bool _isNotPartPayment;

        /// <summary>
        /// In case of part payment, is cash selected
        /// </summary>
        [ObservableProperty]
        private bool _isCashForPart;

        /// <summary>
        /// In case of part payment, is card selected
        /// </summary>
        [ObservableProperty]
        private bool _isCardForPart;

        /// <summary>
        /// In case of part payment, is online selected
        /// </summary>
        [ObservableProperty]
        private bool _isOnlineForPart;

        /// <summary>
        /// In case of part payment, how much is paid in cash
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInCash;

        /// <summary>
        /// In case of part payment, how much is paid in card
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInCard;

        /// <summary>
        /// In case of part payment, how much is paid in online
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInOnline;

        /// <summary>
        /// To kno if the order used gst or not
        /// </summary>
        [ObservableProperty]
        private bool _usingGST;

        /// <summary>
        /// To know if the order type is pickup or not
        /// </summary>
        [ObservableProperty]
        private bool _isPickup;

        /// <summary>
        /// To know if the order type is pickup or not
        /// </summary>
        [ObservableProperty]
        private bool _showReference;

        /// <summary>
        /// To know where the order came from
        /// </summary>
        [ObservableProperty]
        private string _pickupSource;

        /// <summary>
        /// To know who took the order for delivery
        /// </summary>
        [ObservableProperty]
        private string _pickupDelivery;

        /// <summary>
        /// To know the reference number in case of online order
        /// </summary>
        [ObservableProperty]
        private string _referenceNumber;

        /// <summary>
        /// Constructor for the OrdersViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="taxService">DI for TaxService</param>
        public OrdersViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// To know that there are some changes in order details
        /// </summary>
        /// <param name="orderChangedMessage">Order Details are changed</param>
        public void Receive(OrderChangedMessage orderChangedMessage)
        {
            GetOrdersAsync();
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
                if (_isInitialized)
                {
                    SelectedType = OrderTypes[0];
                    return;
                }

                _isInitialized = true;
                IsLoading = true;

                foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<OrderTypes>())
                {
                    OrderTypes.Add(desc);
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Orders Screen", "OK");
            }
        }

        /// <summary>
        /// To clear the orders list and add new or updated orders
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async ValueTask GetOrdersAsync()
        {
            try
            {
                Orders.Clear();
                var filteredOrders = await _databaseService.GetFilteredOrderssAsync(SelectedDate, SelectedType != null ? SelectedType.Key : 0);

                if (filteredOrders.Length == 0)
                {
                    await Shell.Current.DisplayAlert("Search Error", "No orders on this date", "Ok");
                    return;
                }

                int startIndex = (_currentPage - 1) * PageSize;
                var paginatedOrders = filteredOrders.Skip(startIndex).Take(PageSize);

                foreach (var o in paginatedOrders)
                {
                    Orders.Add(OrderModel.FromEntity(o));
                }

                CurrentPageLabel = $"Page: {_currentPage}";
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-GetOrdersAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Getting Order Details", "OK");
            }
        }

        /// <summary>
        /// To search the order with given paramters
        /// </summary>
        [RelayCommand]
        private async void Search()
        {
            if (SelectedType == null)
            {
                await Shell.Current.DisplayAlert("Search Error", "Select a order type", "Ok");
                return;
            }

            _currentPage = 1;
            await GetOrdersAsync();
        }

        /// <summary>
        /// Command to be called when user wants to navigate to previous page
        /// </summary>
        [RelayCommand]
        private async void PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await GetOrdersAsync();
            }
        }

        /// <summary>
        /// Command to be called when user wants to navigate to next page
        /// </summary>
        [RelayCommand]
        private async void NextPage()
        {
            try
            {
                var totalOrders = (await _databaseService.GetFilteredOrderssAsync(SelectedDate, SelectedType.Key)).Length;
                if (_currentPage * PageSize < totalOrders)
                {
                    _currentPage++;
                    await GetOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-NextPage Error", ex);
            }
        }

        /// <summary>
        /// Command to get the items of selected order
        /// </summary>
        /// <param name="orderModel">Selected Order</param>
        /// <returns>Returns Task Object</returns>
        [RelayCommand]
        private async Task SelectOrderAsync(OrderModel? orderModel)
        {
            try
            {
                ShowDiscountVariables = false;
                IsPartPayment = false;
                if (orderModel == null || orderModel.Id == 0)
                {
                    OrderItems = [];
                    return;
                }

                var prevSelectedOrder = Orders.FirstOrDefault(o => o.IsSelected);
                if (prevSelectedOrder != null)
                {
                    prevSelectedOrder.IsSelected = false;
                    if (prevSelectedOrder.Id == orderModel.Id)
                    {
                        OrderItems = [];

                        OrderDetailsVisible = false;
                        return;
                    }
                }

                IsLoading = true;
                orderModel.IsSelected = true;
                IsPickup = orderModel.OrderType == Data.OrderTypes.Pickup ? true : false;
                // Get Order KOTs for More Details
                OrderKOTs = (await _databaseService.GetOrderKotsAsync(orderModel.Id))
                                .Select(KOTModel.FromEntity)
                                .ToList();

                OrderKOTIds = string.Join(',', OrderKOTs.Select(o => o.Id).ToArray());

                /*
                 * Get Order KOT Items
                 * Group them together
                 * Calculcate totals
                 */
                var kotItems = new List<KOTItemModel>();
                OrderKOTItems.Clear();

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
                TotalQuantity = orderModel.TotalItemCount;
                SubTotal = orderModel.TotalAmount;
                SubTotalAfterDiscount = SubTotal;

                if (orderModel.IsDiscountGiven)
                {
                    ShowDiscountVariables = true;
                    if (orderModel.IsFixedBased)
                    {
                        DiscountAmount = orderModel.DiscountFixed;
                    }
                    else if (orderModel.IsPercentageBased)
                    {
                        DiscountAmount = SubTotal * orderModel.DiscountPercentage / 100;
                    }
                    SubTotalAfterDiscount = SubTotal - DiscountAmount;
                }

                UsingGST = orderModel.UsingGST;
                CGST = orderModel.CGST;
                SGST = orderModel.SGST;

                CGSTAmount = orderModel.CGSTAmount;
                SGSTAmount = orderModel.SGSTAmount;

                GrandTotal = orderModel.GrandTotal;
                RoundOff = orderModel.RoundOff;

                ShowReference = false;
                if (IsPickup)
                {
                    PickupSource = EnumExtensions.GetDescription((PickupSources)orderModel.Source);
                    PickupDelivery = await _databaseService.StaffOperaiotns.GetStaffNameBasedOnId(orderModel.DeliveryPerson);

                    if (PickupSource == EnumExtensions.GetDescription(PickupSources.Swiggy) || PickupSource == EnumExtensions.GetDescription(PickupSources.Zomato))
                        ShowReference = true;

                    ReferenceNumber = orderModel.ReferenceNo;
                }
                else
                {
                    WaiterName = await _databaseService.StaffOperaiotns.GetStaffNameBasedOnId(orderModel.WaiterId);
                    TableNo = await _databaseService.TableOperations.GetTableNoAsync(orderModel.TableId);
                }

                OrderToShow = orderModel;

                var orderPayment = await _databaseService.OrderPaymentOperations.GetOrderPaymentById(orderModel.Id);

                if (orderPayment != null)
                {
                    PaymentMode = EnumExtensions.GetDescription(orderPayment.PaymentMode);

                    if (orderPayment.PaymentMode == PaymentModes.Part)
                    {
                        IsPartPayment = true;
                        IsCashForPart = orderPayment.IsCashForPart;
                        IsCardForPart = orderPayment.IsCardForPart;
                        IsOnlineForPart = orderPayment.IsOnlineForPart;
                        PaidByCustomerInCash = orderPayment.PartPaidInCash;
                        PaidByCustomerInCard = orderPayment.PartPaidInCard;
                        PaidByCustomerInOnline = orderPayment.PartPaidInOnline;
                    }
                }

                OrderDetailsVisible = true;
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-SelectOrderAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Order", "OK");
            }
        }
        
        /// <summary>
        /// Command to clear all the cart items
        /// </summary>
        [RelayCommand]
        private void ClearCart()
        {
            if (OrderItems.Length == 0)
                return;

            foreach (var order in Orders)
            {
                order.IsSelected = false;
            }

            OrderItems = [];
        }

        #region KOT Flow

        /// <summary>
        /// Command to place an order
        /// </summary>
        /// <param name="cartItems">List of items in the cart</param>
        /// <param name="tableModel">Table for which we are putting the order</param>
        /// <param name="orderType">OrderType coming from UI</param>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> PlaceKOTAsync(CartItemModel[] cartItems, TableModel tableModel, OrderTypes orderType, StaffModel selectedWaiter)
        {
            try
            {
                var kotItems = cartItems.Select(o => new KOTItem
                {
                    Icon = o.Icon,
                    ItemId = o.ItemId,
                    Name = o.Name,
                    Price = o.Price,
                    Quantity = o.Quantity
                }).ToArray();

                var kotModel = new KOTModel
                {
                    KOTDateTime = DateTime.Now,
                    TotalItemCount = cartItems.Sum(x => x.Quantity),
                    TotalPrice = cartItems.Sum(x => x.Amount),
                    Items = kotItems
                };

                List<KOTModel> kots = new List<KOTModel>();
                kots.Add(kotModel);

                string? errorMessage;

                if (tableModel.RunningOrderId != 0)
                {
                    var lastKOTNumber = await _databaseService.GetLastKOTNumberForOrderId(tableModel.RunningOrderId);

                    for (int i = 0; i < kots.Count; i++)
                    {
                        lastKOTNumber += 1;
                        kots[i].KOTNumber = lastKOTNumber;
                    }

                    // existing order, add kot
                    errorMessage = await _databaseService.InsertOrderKOTAsync(kots.ToArray(), tableModel.RunningOrderId);

                    if (errorMessage != null)
                    {
                        await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                        return false;
                    }
                }
                else
                {
                    var lastOrderNumber = await _databaseService.GetLastestOrderNumberForToday();

                    // new order, place order
                    var orderModel = new OrderModel
                    {
                        TableId = tableModel.Id,
                        OrderDate = DateTime.Now,
                        TotalItemCount = kots.Sum(x => x.TotalItemCount),
                        TotalAmount = kots.Sum(x => x.TotalPrice),
                        KOTs = kots.ToArray(),
                        OrderStatus = TableOrderStatus.Running,
                        OrderNumber = lastOrderNumber + 1,
                        OrderType = orderType,
                        NumberOfPeople = tableModel.NumberOfPeople,
                        WaiterId = selectedWaiter.Id,
                        GrandTotal = kots.Sum(x => x.TotalPrice),
                    };

                    errorMessage = await _databaseService.PlaceOrderAsync(orderModel);

                    if (errorMessage != null)
                    {
                        await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                        return false;
                    }

                    tableModel.RunningOrderId = orderModel.Id;
                    tableModel.Status = TableOrderStatus.Running;

                    // To maintain state
                    tableModel.StartTime = DateTime.Now;
                    WeakReferenceMessenger.Default.Send(OrderChangedMessage.From(orderModel));
                }

                WeakReferenceMessenger.Default.Send(TableStateChangedMessage.From(tableModel));

                await Shell.Current.DisplayAlert("Success", "Order Placeed Successfully", "Ok");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-PlaceKOTAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Placing Order KOT", "OK");
                return false;
            }
        }

        /// <summary>
        /// Command to place a pickup order
        /// </summary>
        /// <param name="cartItems">List of items in the cart</param>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> PlacePickupAsync(OrderModel orderModel)
        {
            try
            {
                var errorMessage = await _databaseService.PlacePickupOrderAsync(orderModel);

                if (errorMessage != null)
                {
                    await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                    return false;
                }

                await Shell.Current.DisplayAlert("Success", "Order Placeed Successfully", "Ok");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrdersVM-PlacePickupAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Placing Pickup Order", "OK");
                return false;
            }
        }

        #endregion
    }
}
