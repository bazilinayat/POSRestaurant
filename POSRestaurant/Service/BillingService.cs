using CommunityToolkit.Mvvm.Input;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Service
{
    /// <summary>
    /// The billing service to make the model and start printing
    /// </summary>
    public class BillingService
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
        /// DIed ReceiptService
        /// </summary>
        private readonly ReceiptService _receiptService;

        /// <summary>
        /// DIed Setting
        /// </summary>
        private readonly Setting _settingService;

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public List<KOTModel> OrderKOTs { get; set; } = new();

        /// <summary>
        /// To store the order details
        /// </summary>
        public OrderModel OrderModel { get; set; }

        /// <summary>
        /// To use for order details
        /// </summary>
        public TableModel TableModel { get; set; }

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public List<KOTItemBillModel> OrderKOTItems { get; set; } = new();

        /// <summary>
        /// Comma separated order kot ids for this order
        /// </summary>
        private string OrderKOTIds;

        /// <summary>
        /// To decide to show or not the discount variables
        /// </summary>
        private bool ShowDiscountVariables;

        /// <summary>
        /// To know the discount amount
        /// </summary>
        private decimal DiscountAmount;

        /// <summary>
        /// Constructor for the BillingService
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="logger">DI for LogService</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        /// <param name="receiptService">DI for ReceiptService</param>
        public BillingService(DatabaseService databaseService, LogService logger, OrdersViewModel ordersViewModel,
            Setting settingService, ReceiptService receiptService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _settingService = settingService;
            _receiptService = receiptService;
        }

        /// <summary>
        /// To gather information from different sources 
        /// Print the final bill for the customer
        /// </summary>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public async Task PrintBill(TableModel tableModel)
        {
            OrderKOTs.Clear();
            OrderKOTItems.Clear();
            OrderKOTIds = "";
            ShowDiscountVariables = false;

            TableModel = tableModel;

            await GetOrderDetailsAsync();
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task GetOrderDetailsAsync()
        {
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

                await PrintReceiptAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("BillingService-GetOrderDetailsAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Bill Details", "OK");
            }
        }

        /// <summary>
        /// Command to call when we want to print the receipt
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task PrintReceiptAsync()
        {
            await Task.Delay(10); // Give UI time to update
            try
            {
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
                    Cashier = TableModel.Cashier.Name,
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
            }
            catch (Exception ex)
            {
                _logger.LogError("BillVM-PrintReceiptAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Printing the Bill", "OK");
            }
        }
    }
}
