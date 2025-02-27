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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for OrderView Page
    /// </summary>
    public partial class OrderViewViewModel : ObservableObject
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
        /// DIed variable for TaxService
        /// </summary>
        private readonly TaxService _taxService;

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
        /// To store the order items of selected order
        /// </summary>
        [ObservableProperty]
        private KOTModel[] _orderKOTs = [];

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
        /// To track the percentage set on UI
        /// Made observable for using in UI
        /// </summary>
        [ObservableProperty, NotifyPropertyChangedFor(nameof(DiscountPercentageAmount))]
        private decimal _discountAmount;

        /// <summary>
        /// To track the fixed discount given by user
        /// </summary>
        [ObservableProperty]
        private decimal _discountFixed;

        /// <summary>
        /// To keep track of the tax amount
        /// </summary>
        public decimal DiscountPercentageAmount => (SubTotal * DiscountAmount) / 100;

        /// <summary>
        /// To keep track of the total amount of the bill
        /// </summary>
        [ObservableProperty]
        private decimal _total;

        /// <summary>
        /// ObservableCollection for items added to cart
        /// </summary>
        public ObservableCollection<KOTItemModel> OrderItems { get; set; } = new();

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
        /// To see if there are any changes done in the order
        /// </summary>
        private bool IsUpdated = false;

        /// <summary>
        /// To keep track of the changes done to the KOT items
        /// </summary>
        private List<KOTItemModel> _removedItems = new();

        /// <summary>
        /// To keep track of the updated KOTItems
        /// </summary>
        private List<long> _updatedKOTItemIds = new();

        /// <summary>
        /// To use for order details
        /// </summary>
        public TableModel TableModel { get; set; }

        /// <summary>
        /// To store the order details
        /// </summary>
        [ObservableProperty]
        public OrderModel _orderModelToDisplay;

        /// <summary>
        /// To be set True, if discount is given
        /// </summary>
        private bool IsDiscountGiven {  get; set; }

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
        /// To keep track of the table number
        /// </summary>
        [ObservableProperty]
        public int _tableNumber;

        /// <summary>
        /// To know different discount types in picker
        /// </summary>
        public ObservableCollection<ValueForPicker> DiscountOptionsTS { get; set; } = new();

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
                            CalculateTotal();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("OrderViewVM-SelectedDiscountType Set Error", ex);
                    }
                }
            }
        }

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
        public OrderViewViewModel(LogService logger, DatabaseService databaseService, MenuService menuService, 
            OrdersViewModel ordersViewModel, Setting settingService,
            TaxService taxService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _settingService = settingService;
            _menuService = menuService;
            _taxService = taxService;
            OrderItems.CollectionChanged += CartItems_CollectionChanged;

            Name = _settingService.Settings.CustomerName;
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
                IsLoading = true;

                TableNumber = TableModel.TableNo;

                OrderItems.Clear();

                Categories = await _menuService.GetMenuCategories();

                Categories[0].IsSelected = true;
                SelectedCategory = Categories[0];

                MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

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

                await GetOrderDetailsAsync();
                await GetOrderKOTsAsync();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Order View Screen", "OK");
            }
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task GetOrderDetailsAsync()
        {
            try
            {
                var order = await _databaseService.GetOrderById(TableModel.RunningOrderId);

                OrderModelToDisplay = new OrderModel
                {
                    Id = order.Id,
                    TableId = order.TableId,
                    OrderDate = order.OrderDate,
                    TotalItemCount = order.TotalItemCount,
                    TotalAmount = order.TotalAmount,
                    PaymentMode = order.PaymentMode,
                    OrderStatus = order.OrderStatus,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-GetOrderDetailsAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Order Details", "OK");
            }
        }

        /// <summary>
        /// Command to get the items of selected order
        /// </summary>
        /// <returns>Returns Task Object</returns>
        private async Task GetOrderKOTsAsync()
        {
            try
            {
                IsLoading = true;

                OrderKOTs = (await _databaseService.GetOrderKotsAsync(OrderModelToDisplay.Id))
                                .Select(KOTModel.FromEntity)
                                .ToArray();

                foreach (var kot in OrderKOTs)
                {
                    var items = (await _databaseService.GetKotItemsAsync(kot.Id))
                                .Select(KOTItemModel.FromEntity)
                                .ToList();

                    foreach (var item in items)
                        OrderItems.Add(item);
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-GetOrderKOTsAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Getting Order KOTs", "OK");
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
                _logger.LogError("OrderViewVM-SelectCategoryAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading the Selected Category", "OK");
            }
        }

        /// <summary>
        /// Command to add tapped items to cart
        /// </summary>
        /// <param name="menuItem">MenuItem which was tapped</param>
        [RelayCommand]
        private void AddToCart(ItemOnMenu menuItem)
        {
            var cartItem = OrderItems.FirstOrDefault(o => o.ItemId == menuItem.Id);
            if (cartItem == null)
            {
                // Item does not exist in cart, add to cart
                OrderItems.Add(new KOTItemModel()
                {
                    Id = 0,
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
                _updatedKOTItemIds.Add(cartItem.Id);
                IsUpdated = true;
            }
        }

        /// <summary>
        /// Command to increase item quantity
        /// </summary>
        /// <param name="cartItem">Item from cart to increase quantity</param>
        [RelayCommand]
        private void IncreaseQuantity(KOTItemModel cartItem)
        {
            cartItem.Quantity++;
            ReCalculateAmount();
            _updatedKOTItemIds.Add(cartItem.Id);
            IsUpdated = true;
        }

        /// <summary>
        /// Command to decrease item quantity
        /// </summary>
        /// <param name="cartItem">Item from cart to descrease quantity</param>
        [RelayCommand]
        private void DecreaseQuantity(KOTItemModel cartItem)
        {
            cartItem.Quantity--;

            if (cartItem.Quantity == 0)
            {
                OrderItems.Remove(cartItem);
                _removedItems.Add(cartItem);
            }
            else
            {
                _updatedKOTItemIds.Add(cartItem.Id);
                ReCalculateAmount();
            }

            IsUpdated = true;
        }

        /// <summary>
        /// Command to remove item from cart
        /// </summary>
        /// <param name="cartItem">Item from cart to remove</param>
        [RelayCommand]
        private void RemoveItemFromCart(KOTItemModel cartItem)
        {
            OrderItems.Remove(cartItem);
            _removedItems.Remove(cartItem);
        }

        /// <summary>
        /// To recalculate amount when items or quantity changes
        /// </summary>
        private void ReCalculateAmount()
        {
            SubTotal = OrderItems.Sum(o => o.Amount);
            CalculateTotal();
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
                        Total = SubTotal - DiscountAmount;
                    }
                    else if (SelectedDiscountType.Key == (int)DiscountOptions.Percentage)
                    {
                        DiscountAmount = (SubTotal * enteredDiscount) / 100;
                        DiscountAmount = enteredDiscount;
                        Total = SubTotal - DiscountPercentageAmount;
                    }
                }
                IsUpdated = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-DiscountClickAsync Error", ex);
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
                _logger.LogError("OrderViewVM-SearchItems Error", ex);
            }
        }

        /// <summary>
        /// Command to handle SaveAndPrint click
        /// This will update the order if necessary
        /// And save the order and print the receipt
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async void SaveAndPrint()
        {
            /*
             * Here we must first update all the kot items in database
             * For KOT items, with id = 0, which means newly added items, create a new kot
             * Then update the corresponding kots, with new item number and total price
             * Then update the corresponding order, with new item number and total price
             * After all the oprerations, print the receipt
             */

            try
            {
                // Remove the KOTItems from DB and recalculate total
                var toDelete = _removedItems.GroupBy(o => o.KOTId).ToDictionary(group => group.Key, group => group.Select(KOTItem.FromEntity).ToArray());

                if (toDelete.Count > 0)
                {
                    var errorMessage = await _databaseService.DeleteKOTItemsAndUpdateKOT(toDelete, TableModel.RunningOrderId);

                    if (errorMessage != null)
                        await Shell.Current.DisplayAlert("Saving Error", errorMessage, "OK");
                }

                // Update OrderItems => KOTItemModel, where KOTItemId != 0
                var toUpdate = OrderItems.Where(o => o.Id != 0 && _updatedKOTItemIds.Contains(o.Id)).GroupBy(o => o.KOTId).ToDictionary(group => group.Key, group => group.Select(KOTItem.FromEntity).ToArray());

                if (toUpdate.Count > 0)
                {
                    var errorMessage = await _databaseService.UpdateKOTItemsAndKOT(toUpdate, TableModel.RunningOrderId);

                    if (errorMessage != null)
                        await Shell.Current.DisplayAlert("Saving Error", errorMessage, "OK");
                }

                // Add new KOT items if needed
                KOTItem[] toAdd = OrderItems.Where(o => o.Id == 0).Select(KOTItem.FromEntity).ToArray();

                if (toAdd.Length > 0)
                {
                    var lastKOTNumber = await _databaseService.GetLastKOTNumberForOrderId(TableModel.RunningOrderId);

                    var kotModel = new KOTModel
                    {
                        KOTNumber = lastKOTNumber + 1,
                        KOTDateTime = DateTime.Now,
                        TotalItemCount = toAdd.Length,
                        TotalPrice = toAdd.Sum(x => x.Price),
                        Items = toAdd
                    };
                    List<KOTModel> kots = new List<KOTModel>();
                    kots.Add(kotModel);

                    var errorMessage = await _databaseService.InsertOrderKOTAsync(kots.ToArray(), TableModel.RunningOrderId);

                    if (errorMessage != null)
                        await Shell.Current.DisplayAlert("Saving Error", errorMessage, "OK");
                }

                // Add Discounts to Order if any
                if (IsDiscountGiven)
                {
                    Discount discount = new Discount
                    {
                        OrderId = TableModel.RunningOrderId,
                        IsFixedBased = EnableFixedDiscount,
                        IsPercentageBased = EnableDiscount,
                        DiscountFixed = DiscountAmount,
                        DiscountPercentage = DiscountAmount,
                    };
                    await _databaseService.DiscountOperations.SaveDiscountAsync(discount);
                }

                if (await UpdateOrder() <= 0)
                {
                    await Shell.Current.DisplayAlert("Saving Error", "Couldn't Update Order", "OK");
                    return;
                }

                TableModel.Status = TableOrderStatus.Confirmed;
                // Push for change in table info
                WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-SaveAndPrint Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving Updated Order", "OK");
            }
        }

        /// <summary>
        /// To update the order in db and add all the necessary information there
        /// </summary>
        /// <returns>Return the number of rows affected by the operation</returns>
        private async Task<int> UpdateOrder()
        {
            try
            {
                var order = await _databaseService.GetOrderById(TableModel.RunningOrderId);

                order.IsDiscountGiven = IsDiscountGiven;
                order.IsFixedBased = SelectedDiscountType.Key == (int)DiscountOptions.Fixed ? true : false;
                order.IsPercentageBased = SelectedDiscountType.Key == (int)DiscountOptions.Percentage ? true : false; ;
                order.DiscountFixed = DiscountAmount;
                order.DiscountPercentage = DiscountAmount;
                if (IsDiscountGiven)
                {
                    if (SelectedDiscountType.Key == (int)DiscountOptions.Fixed)
                    {
                        order.TotalAmountAfterDiscount = order.TotalAmount - DiscountAmount;
                    }
                    else if (SelectedDiscountType.Key == (int)DiscountOptions.Percentage)
                    {
                        var amount = order.TotalAmount * DiscountAmount / 100;
                        order.TotalAmountAfterDiscount = order.TotalAmount - amount;
                    }
                }
                else
                {
                    order.TotalAmountAfterDiscount = order.TotalAmount;
                }


                order.UsingGST = _taxService.IndianTaxService.UsingGST;
                order.CGST = _taxService.IndianTaxService.CGST;
                order.SGST = _taxService.IndianTaxService.SGST;
                order.CGSTAmount = _taxService.IndianTaxService.CalculateCGST(order.TotalAmount);
                order.SGSTAmount = _taxService.IndianTaxService.CalculateSGST(order.TotalAmount);

                if (order.UsingGST)
                {
                    var total = order.TotalAmountAfterDiscount + order.CGSTAmount + order.SGSTAmount;
                    order.GrandTotal = Math.Floor(total);
                    order.RoundOff = order.GrandTotal - total;
                }
                else
                {
                    order.GrandTotal = Math.Floor(order.TotalAmountAfterDiscount);
                    order.RoundOff = order.GrandTotal - order.TotalAmountAfterDiscount;
                }

                return await _databaseService.UpdateOrder(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderViewVM-UpdateOrder Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Updating Order", "OK");
                return 0;
            }
        }

        /// <summary>
        /// To calculate the total amount
        /// </summary>
        private void CalculateTotal()
        {
            if (SelectedDiscountType == null) return;
            Total = SubTotal;
            if (SelectedDiscountType.Key == 1)
            {
                Total = SubTotal - DiscountPercentageAmount;
            }
            else if (SelectedDiscountType.Key == 2)
            {
                Total = SubTotal - DiscountFixed;
            }
        }
    }
}
