using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.PaymentService.Online;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.PaymentService;
using POSRestaurant.Service.SettingService;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using ZXing;
using ZXing.Common;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Home Page
    /// </summary>
    public partial class PickupViewModel : ObservableObject, IRecipient<MenuItemChangedMessage>, IRecipient<StaffChangedMessage>
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
        /// DIed SettingService
        /// </summary>
        private readonly TaxService _taxService;

        /// <summary>
        /// DIed ReceiptService
        /// </summary>
        private readonly ReceiptService _receiptService;

        /// <summary>
        /// DIed RazorPayService
        /// </summary>
        private readonly RazorPayService _razorPayService;

        /// <summary>
        /// DIed PaymentMonitoringService
        /// </summary>
        private readonly PaymentMonitoringService _paymentMonitoringService;

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
        /// To get and display all the categories
        /// Made observable to use everywhere
        /// </summary>
        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        /// <summary>
        /// To get and display all the items from category
        /// Made observable to use everywhere
        /// </summary>
        [ObservableProperty]
        private ItemOnMenu[] _menuItems = [];

        /// <summary>
        /// Property to observe the selected category on UI
        /// </summary>
        [ObservableProperty]
        private MenuCategoryModel _selectedCategory;

        /// <summary>
        /// To track the sub total of the order or KOT
        /// Made observable for using
        /// </summary>
        [ObservableProperty, NotifyPropertyChangedFor(nameof(Total))]
        private decimal _subTotal;

        /// <summary>
        /// To track the name for UI
        /// Made observable for using in UI
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// To keep track of the total amount of the bill
        /// </summary>
        public decimal Total => SubTotal;

        /// <summary>
        /// ObservableCollection for items added to cart
        /// </summary>
        public ObservableCollection<CartItemModel> CartItems { get; set; } = new();

        /// <summary>
        /// DIed OrdersViewModel
        /// </summary>
        private readonly OrdersViewModel _ordersViewModel;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly Setting _settingService;

        /// <summary>
        /// ObservableProperty for the SearchBox, to search for items
        /// </summary>
        [ObservableProperty]
        private string _textSearch;

        /// <summary>
        /// List of sources to be assigned to the order
        /// </summary>
        [ObservableProperty]
        private ValueForPicker[] _pickupSources;

        /// <summary>
        /// To manage the selected waiter for the order
        /// </summary>
        private ValueForPicker _selectedSource;

        /// <summary>
        /// To manage the selection of waiter on main page
        /// Should be assigned by code as well
        /// </summary>
        public ValueForPicker SelectedSource
        {
            get => _selectedSource;
            set
            {
                if (_selectedSource != value)
                {
                    _selectedSource = value;

                    if (_selectedSource != null) {
                        if (_selectedSource.Key == (int)Data.PickupSources.Swiggy || _selectedSource.Key == (int)Data.PickupSources.Zomato)
                            ReferenceNeeded = true;
                        else
                            ReferenceNeeded = false;
                    }
                    else
                    {
                        ReferenceNeeded = false;
                    }

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// To know if reference number is needed for this order
        /// </summary>
        [ObservableProperty]
        private bool _referenceNeeded;

        /// <summary>
        /// In case swiggy or zomato is selected, we need to know the reference number linking the two platforms
        /// </summary>
        [ObservableProperty]
        private string _referenceNo;

        /// <summary>
        /// The different delivery persons which are available for delivery
        /// </summary>
        [ObservableProperty]
        private List<StaffModel> _deliveryPersons;

        /// <summary>
        /// The selected delivery person for this order
        /// </summary>
        [ObservableProperty]
        private StaffModel _selectedDeliveryPerson;
            
        /// <summary>
        /// To know the total item count for this order
        /// </summary>
        [ObservableProperty]
        private int _totalItemCount;

        /// <summary>
        /// The total amount of the order
        /// </summary>
        [ObservableProperty]
        private decimal _totalAmount;

        /// <summary>
        /// To be set True, if discount is given
        /// </summary>
        [ObservableProperty]
        private bool _isDiscountGiven;

        /// <summary>
        /// To know different discount types in picker
        /// </summary>
        public ObservableCollection<ValueForPicker> DiscountOptionsTS { get; set; } = new();

        /// <summary>
        /// To enable or disable percentage discount
        /// </summary>
        [ObservableProperty]
        private bool _enableDiscount;

        /// <summary>
        /// To enable or disable fixed discount
        /// </summary>
        [ObservableProperty]
        private bool _enableFixedDiscount;

        /// <summary>
        /// Selected type for filter
        /// </summary>
        private ValueForPicker _selectedDiscountType;

        /// <summary>
        /// To manage the selected order type on main page
        /// Should be handled by code as well
        /// </summary>
        public ValueForPicker SelectedDiscountType
        {
            get => _selectedDiscountType;
            set
            {
                if (_selectedDiscountType != value)
                {
                    try
                    {
                        _selectedDiscountType = value;
                        if (_selectedDiscountType == null)
                        {
                            IsDiscountGiven = false;
                            DiscountAmount = 0;
                            EnableDiscount = false;
                            OnPropertyChanged();
                            return;
                        }
                        if (_selectedDiscountType.Key == 0)
                        {
                            IsDiscountGiven = false;
                            DiscountAmount = 0;
                            EnableDiscount = false;
                        }
                        else if (_selectedDiscountType.Key == 1 || _selectedDiscountType.Key == 2)
                        {
                            DiscountAmount = 0;
                            EnableDiscount = true;
                            IsDiscountGiven = true;
                        }
                        OnPropertyChanged();
                        if (_selectedDiscountType.Key != 0)
                            ReCalculateAmount();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("PickupVM-SelectedDiscountType Set Error", ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// The amount to substract from total
        /// </summary>
        [ObservableProperty]
        private decimal _discountAmount;

        /// <summary>
        /// Total amount after substracting the discount
        /// </summary>
        [ObservableProperty]
        private decimal _totalAmountAfterDiscount;

        /// <summary>
        /// To know if the restaurant is using gst
        /// </summary>
        [ObservableProperty]
        private bool _usingGst;

        /// <summary>
        /// To show the cgst percentage
        /// </summary>
        [ObservableProperty]
        private decimal _cgst;

        /// <summary>
        /// To show the sgst percentage
        /// </summary>
        [ObservableProperty]
        private decimal _sgst;

        /// <summary>
        /// To show the cgst amount
        /// </summary>
        [ObservableProperty]
        private decimal _cgstAmount;

        /// <summary>
        /// To show the sgst amount
        /// </summary>
        [ObservableProperty]
        private decimal _sgstAmount;

        /// <summary>
        /// The round off which will be substracted
        /// </summary>
        [ObservableProperty]
        private decimal _roundOff;

        /// <summary>
        /// The grand total of the order
        /// </summary>
        [ObservableProperty]
        private decimal _grandTotal;

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public List<KOTModel> OrderKOTs { get; set; } = new();

        /// <summary>
        /// Comma separated order kot ids for this order
        /// </summary>
        private string OrderKOTIds;

        /// <summary>
        /// To know the order id of this pickup order
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// The cashier who handled the order
        /// </summary>
        public StaffModel Cashier { get; set; }

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        public ObservableCollection<KOTItemBillModel> OrderKOTItems { get; set; } = new();

        /// <summary>
        /// To handle the property changed event for the radio button switch
        /// </summary>
        public event PropertyChangedEventHandler SomePropertyChanged;

        /// <summary>
        /// Called when OrderType is changed
        /// </summary>
        /// <param name="orderType">Selected OrderType name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string orderType = null)
        {
            SomePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(orderType));
        }

        /// <summary>
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public PickupViewModel(LogService logger, DatabaseService databaseService, 
            MenuService menuService, OrdersViewModel ordersViewModel, 
            Setting settingService, TaxService taxService,
            ReceiptService receiptService, RazorPayService razorPayService,
            PaymentMonitoringService paymentMonitoringService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _menuService = menuService;
            _taxService = taxService;
            _ordersViewModel = ordersViewModel;
            _settingService = settingService;
            _receiptService = receiptService;
            _razorPayService = razorPayService;
            _paymentMonitoringService = paymentMonitoringService;

            CartItems.CollectionChanged += CartItems_CollectionChanged;

            Name = _settingService.Settings.CustomerName;

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<MenuItemChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<StaffChangedMessage>(this);
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
                UsingGst = _taxService.IndianTaxService.UsingGST;
                Cgst = _taxService.IndianTaxService.CGST;
                Sgst = _taxService.IndianTaxService.SGST;
                IsDiscountGiven = false;
                DiscountAmount = 0;
                CartItems.Clear();

                DiscountOptionsTS.Clear();
                foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<DiscountOptions>())
                {
                    DiscountOptionsTS.Add(desc);
                }
                var defaultItem = DiscountOptionsTS.FirstOrDefault(x => x.Key == 0);
                if (defaultItem != null)
                {
                    SelectedDiscountType = defaultItem;
                }
                _selectedDiscountType = DiscountOptionsTS.First();

                if (_isInitialized)
                {
                    foreach (var category in Categories)
                    {
                        category.IsSelected = false;
                    }
                    Categories[0].IsSelected = true;
                    SelectedCategory = Categories[0];
                    MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

                    return;
                }

                _isInitialized = true;

                IsLoading = true;

                Categories = await _menuService.GetMenuCategories();

                Categories[0].IsSelected = true;
                SelectedCategory = Categories[0];

                MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

                PickupSources = EnumExtensions.GetAllDescriptions<PickupSources>().ToArray();

                await LoadDeliveryPersons();

                CartItems.Clear();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Pickup Menu", "OK");
            }
        }

        /// <summary>
        /// Refresh staff details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(StaffChangedMessage message)
        {
            await LoadDeliveryPersons();
        }

        /// <summary>
        /// Command to open a dialog box for accepting tax percentage
        /// </summary>
        /// <returns>A Task object</returns>
        [RelayCommand]
        private async Task DiscountClickAsync()
        {
            try
            {
                var result = await Shell.Current.DisplayPromptAsync("Discount", "Enter the applicable discount in percentage / fixed amount.", placeholder: "10", initialValue: DiscountAmount.ToString());
                if (!string.IsNullOrWhiteSpace(result))
                {

                    if (!Decimal.TryParse(result, out decimal enteredDiscount))
                    {
                        await Shell.Current.DisplayAlert("Invalid Value", "Entered discount is invalid.", "Ok");
                        return;
                    }

                    if (SelectedDiscountType.Key == (int)DiscountOptions.Fixed)
                    {
                        DiscountAmount = enteredDiscount;
                        ReCalculateAmount();
                    }
                    else if (SelectedDiscountType.Key == (int)DiscountOptions.Percentage)
                    {
                        DiscountAmount = (SubTotal * enteredDiscount) / 100;
                        DiscountAmount = enteredDiscount;
                        ReCalculateAmount();

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-DiscountClickAsync Error", ex);
            }
        }

        /// <summary>
        /// To call the database and load the list of waiters
        /// </summary>
        /// <returns>Returns a task object</returns>
        private async Task LoadDeliveryPersons()
        {
            try
            {
                DeliveryPersons = (await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.Delivery))
                                    .Select(StaffModel.FromEntity)
                                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-LoadDeliveryPersons Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Delivery Persons", "OK");
            }
        }

        /// <summary>
        /// Change data as per selected category
        /// Working as a relay command
        /// </summary>
        /// <param name="categoryId">CategoryId of the MenuCategory selected</param>
        [RelayCommand]
        private async Task SelectCategoryAsync(int categoryId)
        {
            try
            {
                if (SelectedCategory.Id == categoryId) return;

                IsLoading = true;

                var existingSelectedCategory = Categories.First(o => o.IsSelected);
                existingSelectedCategory.IsSelected = false;

                var newSelectedCategory = Categories.First(o => o.Id == categoryId);
                newSelectedCategory.IsSelected = true;

                SelectedCategory = newSelectedCategory;

                MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-SelectCategoryAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Category", "OK");
            }
        }

        /// <summary>
        /// Command to add tapped items to cart
        /// </summary>
        /// <param name="menuItem">MenuItem which was tapped</param>
        [RelayCommand]
        private void AddToCart(ItemOnMenu menuItem)
        {
            var cartItem = CartItems.FirstOrDefault(o => o.ItemId == menuItem.Id);
            if (cartItem == null)
            {
                // Item does not exist in cart, add to cart
                CartItems.Add(new CartItemModel()
                {
                    ItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Icon = menuItem.Icon,
                    Price = menuItem.Price,
                    Quantity = 1
                });
            }
            else
            {
                // Item already exists in cart, Increase quantity for this item in cart
                cartItem.Quantity++;
                ReCalculateAmount();
            }
        }

        /// <summary>
        /// Command to increase item quantity
        /// </summary>
        /// <param name="cartItem">Item from cart to increase quantity</param>
        [RelayCommand]
        private void IncreaseQuantity(CartItemModel cartItem)
        {
            cartItem.Quantity++;
            ReCalculateAmount();
        }

        /// <summary>
        /// Command to decrease item quantity
        /// </summary>
        /// <param name="cartItem">Item from cart to descrease quantity</param>
        [RelayCommand]
        private void DecreaseQuantity(CartItemModel cartItem)
        {
            cartItem.Quantity--;

            if (cartItem.Quantity == 0)
                CartItems.Remove(cartItem);
            else
                ReCalculateAmount();
        }

        /// <summary>
        /// Command to remove item from cart
        /// </summary>
        /// <param name="cartItem">Item from cart to remove</param>
        [RelayCommand]
        private void RemoveItemFromCart(CartItemModel cartItem) =>
            CartItems.Remove(cartItem);

        /// <summary>
        /// To recalculate amount when items or quantity changes
        /// </summary>
        private void ReCalculateAmount()
        {
            try
            {
                var totalItems = CartItems.Sum(o => o.Quantity);
                var totalAmount = CartItems.Sum(o => o.Amount);
                // Calculate totals
                TotalItemCount = totalItems;
                TotalAmount = totalAmount;
                TotalAmountAfterDiscount = SubTotal;

                if (IsDiscountGiven)
                {
                    if (SelectedDiscountType.Key == (int)DiscountOptions.Fixed)
                    {
                        DiscountAmount = DiscountAmount;
                        TotalAmountAfterDiscount = TotalAmount - DiscountAmount;
                    }
                    else if (SelectedDiscountType.Key == (int)DiscountOptions.Percentage)
                    {
                        DiscountAmount = TotalAmount * DiscountAmount / 100;
                        TotalAmountAfterDiscount = TotalAmount - DiscountAmount;
                    }
                }
                else
                {
                    TotalAmountAfterDiscount = TotalAmount;
                }

                if (UsingGst)
                {
                    CgstAmount = _taxService.IndianTaxService.CalculateCGST(TotalAmountAfterDiscount);
                    SgstAmount = _taxService.IndianTaxService.CalculateSGST(TotalAmountAfterDiscount);
                    var total = TotalAmountAfterDiscount + CgstAmount + SgstAmount;
                    GrandTotal = Math.Floor(total);
                    RoundOff = GrandTotal - total;
                }
                else
                {
                    GrandTotal = Math.Floor(TotalAmountAfterDiscount);
                    RoundOff = GrandTotal - TotalAmountAfterDiscount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-ReCalculateAmount Error", ex);
            }
        }

        /// <summary>
        /// Event method when the ObservableCollection is changed.
        /// </summary>
        /// <param name="sender">ObservableCollection</param>
        /// <param name="e">EventArgs</param>
        /// <exception cref="NotImplementedException">ExceptionType</exception>
        private void CartItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ReCalculateAmount();
        }

        /// <summary>
        /// Command to clear all the cart items
        /// </summary>
        [RelayCommand]
        private async Task ClearCart()
        {
            if (CartItems.Count == 0)
                return;

            if (await Shell.Current.DisplayAlert("Clear Cart?", "Do you really want to clear the cart?", "Yes", "No"))
                CartItems.Clear();
        }

        /// <summary>
        /// Command to place an order
        /// </summary>
        /// <param name="isPaidOnline">Coming from UI, which button is clicked</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task PlaceOrderAsync()
        {
            try
            {
                IsLoading = true;

                var kotItems = CartItems.Select(o => new KOTItem
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
                    TotalItemCount = CartItems.Sum(x => x.Quantity),
                    TotalPrice = CartItems.Sum(x => x.Amount),
                    Items = kotItems
                };

                List<KOTModel> kots = new List<KOTModel>();
                kots.Add(kotModel);

                string? errorMessage;

                var lastOrderNumber = await _databaseService.GetLastestPickupOrderNumberForToday();

                // new order, place order
                var orderModel = new OrderModel
                {
                    TableId = 0,
                    OrderDate = DateTime.Now,
                    TotalItemCount = kots.Sum(x => x.TotalItemCount),
                    TotalAmount = kots.Sum(x => x.TotalPrice),
                    KOTs = kots.ToArray(),
                    OrderStatus = TableOrderStatus.Running,
                    OrderNumber = lastOrderNumber + 1,
                    OrderType = Data.OrderTypes.Pickup,
                    NumberOfPeople = 0,
                    WaiterId = 0,

                    IsDiscountGiven = IsDiscountGiven,
                    IsFixedBased = SelectedDiscountType.Key == (int)DiscountOptions.Fixed ? true : false,
                    IsPercentageBased = SelectedDiscountType.Key == (int)DiscountOptions.Percentage ? true : false,
                    DiscountFixed = DiscountAmount,
                    DiscountPercentage = DiscountAmount,
                    TotalAmountAfterDiscount = TotalAmountAfterDiscount,

                    UsingGST = UsingGst,
                    CGST = Cgst,
                    SGST = Sgst,
                    CGSTAmount = CgstAmount,
                    SGSTAmount = SgstAmount,

                    RoundOff = RoundOff,
                    GrandTotal = GrandTotal,

                    Source = SelectedSource.Key,
                    ReferenceNo = ReferenceNo,
                    DeliveryPerson = SelectedDeliveryPerson.Id
                };

                if (await _ordersViewModel.PlacePickupAsync(orderModel))
                {
                    OrderId = orderModel.Id;
                }

                //await SaveOrderPaymentAsync();

                var (imageUrl, qrCodeId) = await _razorPayService.GenerateDynamicQR(orderModel.Id, orderModel.GrandTotal);

                // var imageUrl = "https://rzp.io/i/BWcUVrLp";
                var qrContent = await GetQRContent(imageUrl);

                await PrintReceipt(orderModel, qrContent);

                CartItems.Clear();

                WeakReferenceMessenger.Default.Send(OrderChangedMessage.From(orderModel));

                IsLoading = false;

                _paymentMonitoringService.QrCodes.TryAdd(qrCodeId, new PaymentNecessaryDetail
                {
                    QrCodeId = qrCodeId,
                    Orderid = orderModel.Id,
                    OrderTotal = orderModel.GrandTotal,
                    OrderType = orderModel.OrderType,
                });

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-PlaceOrderAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Placing Pickup Order", "OK");
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
        /// <param name="orderModel">Orderdetails of the Order</param>
        /// <param name="qrContent"></param>
        /// <returns>Returns a Task Object</returns>
        private async Task PrintReceipt(OrderModel orderModel, string? qrContent)
        {
            try
            {
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

                foreach (var kot in OrderKOTs)
                {
                    var items = (await _databaseService.GetKotItemsAsync(kot.Id))
                                .Select(KOTItemModel.FromEntity)
                                .ToList();

                    kotItems.AddRange(items);
                }

                // Group items together
                var dict = kotItems.GroupBy(o => o.ItemId).ToDictionary(g => g.Key, g => g.Select(o => o));

                OrderKOTItems.Clear();
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

                var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

                var billModel = new BillModel
                {
                    RestrauntName = restaurantInfo.Name,
                    Phone = restaurantInfo.PhoneNumber,
                    Address = restaurantInfo.Address,
                    GSTIn = restaurantInfo.GSTIN,
                    CustomerName = "Customer Name",

                    OrderType = orderModel.OrderType,

                    TimeStamp = orderModel.OrderDate,
                    TableNo = orderModel.TableId,
                    Cashier = Cashier.Name,
                    BillNo = orderModel.Id.ToString(),
                    TokenNos = OrderKOTIds,
                    WaiterAssigned = "",

                    Items = OrderKOTItems.ToList(),

                    TotalQty = orderModel.TotalItemCount,
                    SubTotal = orderModel.TotalAmount,

                    IsDiscountGiven = orderModel.IsDiscountGiven,
                    IsFixedBased = orderModel.IsFixedBased,
                    IsPercentageBased = orderModel.IsPercentageBased,
                    DiscountFixed = orderModel.DiscountFixed,
                    DiscountPercentage = orderModel.DiscountPercentage,
                    SubTotalAfterDiscount = orderModel.TotalAmountAfterDiscount,

                    UsginGST = orderModel.UsingGST,
                    CGST = orderModel.CGST,
                    SGST = orderModel.SGST,
                    CGSTAmount = orderModel.CGSTAmount,
                    SGSTAmount = orderModel.SGSTAmount,
                    RoundOff = orderModel.RoundOff,
                    GrandTotal = orderModel.GrandTotal,

                    FassaiNo = restaurantInfo.FSSAI,
                    QRCode = qrContent != null ? qrContent : "Data",

                    Source = SelectedSource.Value,
                    ReferenceNo = ReferenceNo,
                    DeliveryPersonName = SelectedDeliveryPerson.Name
                };

                var pdfData = await _receiptService.GenerateReceipt(billModel);
                await _receiptService.PrintReceipt(pdfData);
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-PrintReceipt Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Printing Receipt", "OK");
            }
        }

        /// <summary>
        /// Command to be called when search box changes
        /// </summary>
        /// <param name="textSearch">Query to search</param>
        [RelayCommand]
        private void SearchItems(string? textSearch)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textSearch) || textSearch.Length < 3)
                    return;

                Task.Run(async () =>
                {
                    var result = await _databaseService.GetMenuItemBySearch(textSearch);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MenuItems = result;
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-SearchItems Error", ex);
            }
        }

        /// <summary>
        /// Implemented interface IRecipient
        /// </summary>
        /// <param name="message">ItemOnMenuModel published from other parts of the application</param>
        public void Receive(MenuItemChangedMessage message)
        {
            try
            {
                var changedItem = message.Value;
                var menuItem = changedItem.ItemModel;
                bool isDeleted = changedItem.IsDeleted;

                if (menuItem != null)
                {
                    // This menu item is on the screen right now

                    // check if this item still has a mapping to selected category
                    // can be used for delete part
                    if (SelectedCategory != null)
                    {
                        if (isDeleted && menuItem.Category.Id == SelectedCategory.Id)
                        {
                            // this item is deleted, should not be displayed here anymore
                            // remove this item from the current UI menu items list
                            MenuItems = [.. MenuItems.Where(m => m.Id != menuItem.Id)];
                        }
                    }

                    // update details of existing item on the screen
                    menuItem.Price = menuItem.Price;
                    menuItem.Name = menuItem.Name;
                    menuItem.Description = menuItem.Description;
                    MenuItems = [.. MenuItems];
                }
                else
                {
                    // model is newly added
                    // add this menu item to current UI menu items list
                    var item = new ItemOnMenu
                    {
                        Id = menuItem.Id,
                        Description = menuItem.Description,
                        Name = menuItem.Name,
                        Price = menuItem.Price,
                        MenuCategoryId = menuItem.Category.Id
                    };
                    MenuItems = [.. MenuItems, item];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("PickupVM-Recieve MenuItemChangedMessage Error", ex);
                Shell.Current.DisplayAlert("Fault", "Error while Changing Menu Item", "OK");
            }
        }
    }
}
