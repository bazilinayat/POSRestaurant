using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.PaymentService.Online;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.PaymentService;
using POSRestaurant.Service.SettingService;
using POSRestaurant.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

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
        /// DIed Setting
        /// </summary>
        private readonly RazorPayService _razorPayService;

        /// <summary>
        /// DIed PaymentMonitoringService
        /// </summary>
        private readonly PaymentMonitoringService _paymentMonitoringService;

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
            Setting settingService, ReceiptService receiptService, RazorPayService razorPayService,
            PaymentMonitoringService paymentMonitoringService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _settingService = settingService;
            _receiptService = receiptService;
            _razorPayService = razorPayService;
            _paymentMonitoringService = paymentMonitoringService;
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

                var (imageUrl, qrCodeId) = await _razorPayService.GenerateDynamicQR(OrderModel.Id, OrderModel.GrandTotal);

                // var imageUrl = "https://rzp.io/i/BWcUVrLp";
                var qrContent = await GetQRContent(imageUrl);
                await PrintReceiptAsync(qrContent);

                _paymentMonitoringService.QrCodes.TryAdd(qrCodeId, new PaymentNecessaryDetail
                {
                    QrCodeId = qrCodeId,
                    Orderid = OrderModel.Id,
                    OrderTotal = OrderModel.GrandTotal,
                    OrderType = OrderModel.OrderType,
                    TableModelToUpdate = TableModel
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("BillingService-GetOrderDetailsAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Bill Details", "OK");
            }
        }

        /// <summary>
        /// To download the image and extract the qr code content to make our own
        /// </summary>
        /// <param name="imageUrl">Image url to get the content</param>
        /// <returns>QR code content if success, else null</returns>
        private async Task<string?> GetQRContent(string imageUrl)
        {
            string qrContent = null;
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");

            // Step 1: Download image to temp file
            using (HttpClient client = new HttpClient())
            using (var response = await client.GetAsync(imageUrl))
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fileStream);
            }

            try
            {
                // Step 2: Extract QR code content using ZXing.Net
                qrContent = DecodeQrCode(tempFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError("BillingService-PrintReceiptAsync Error", ex);
            }

            return qrContent;
        }

        /// <summary>
        /// Method to decode the qr code from image and return the scanned content from the qrcode needed
        /// </summary>
        /// <param name="imagePath">Temporary image path</param>
        /// <returns>The scanned qr code content</returns>
        private string DecodeQrCode(string imagePath)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    // Convert bitmap to byte array
                    var luminanceSource = ConvertBitmapToLuminanceSource(bitmap);

                    if (luminanceSource == null)
                    {
                        Console.WriteLine("Failed to convert image to luminance source.");
                        return "";
                    }

                    var binarizer = new HybridBinarizer(luminanceSource);
                    var bitmapMatrix = new BinaryBitmap(binarizer);

                    var reader = new MultiFormatReader();
                    var result = reader.decode(bitmapMatrix);

                    return result?.Text ?? "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding QR Code: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// To convert the downloaded image to a RGB Luminance source for processing further
        /// </summary>
        /// <param name="bitmap">Bitmap image to convert</param>
        /// <returns>The LuminanceSource required to process</returns>
        private LuminanceSource ConvertBitmapToLuminanceSource(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Lock the bitmap data to access pixel bytes
            var rect = new Rectangle(0, 0, width, height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            int stride = bitmapData.Stride;
            byte[] pixelBuffer = new byte[stride * height];

            // Copy pixel data
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(bitmapData);

            // Convert to LuminanceSource for ZXing
            return new RGBLuminanceSource(pixelBuffer, width, height, RGBLuminanceSource.BitmapFormat.RGB32);
        }

        /// <summary>
        /// Command to call when we want to print the receipt
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task PrintReceiptAsync(string? qrContent)
        {
            await Task.Delay(10); // Give UI time to update
            try
            {
                var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

                var billModel = new BillModel
                {
                    RestrauntName = restaurantInfo.Name,
                    Phone = restaurantInfo.PhoneNumber,
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
                    QRCode = qrContent != null ? qrContent : "Data"
                };

                var pdfData = await _receiptService.GenerateReceipt(billModel);
                await _receiptService.PrintReceipt(pdfData);
            }
            catch (Exception ex)
            {
                _logger.LogError("BillingService-PrintReceiptAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Printing the Bill", "OK");
            }
        }
    }
}
