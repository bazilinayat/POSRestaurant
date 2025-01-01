using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for OrderView Page
    /// </summary>
    public partial class ViewOrderViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// DIed variable for MenuService
        /// </summary>
        private readonly MenuService _menuService;

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
        private decimal _discountPercentage;

        /// <summary>
        /// To track the fixed discount given by user
        /// </summary>
        [ObservableProperty]
        private decimal _discountFixed;

        /// <summary>
        /// To keep track of the tax amount
        /// </summary>
        public decimal DiscountPercentageAmount => (SubTotal * DiscountPercentage) / 100;

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
        private readonly SettingService _settingService;

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
        public OrderModel OrderModel { get; set; }

        /// <summary>
        /// To be set True, if discount is given
        /// </summary>
        private bool IsDiscountGiven {  get; set; }

        /// <summary>
        /// To manage the selected order type on main page
        /// </summary>
        private int _selectedDiscountType;

        /// <summary>
        /// To enable or disable percentage discount
        /// </summary>
        [ObservableProperty]
        private bool _enablePercentageDiscount;

        /// <summary>
        /// To enable or disable fixed discount
        /// </summary>
        [ObservableProperty]
        private bool _enableFixedDiscount;

        /// <summary>
        /// To manage the selected order type on main page
        /// Should be handled by code as well
        /// </summary>
        public int SelectedDiscountType
        {
            get => _selectedDiscountType;
            set
            {
                if (_selectedDiscountType != value)
                {
                    _selectedDiscountType = value;

                    if (_selectedDiscountType == 1)
                    {
                        EnablePercentageDiscount = true;
                        EnableFixedDiscount = false;
                    }
                    else if (_selectedDiscountType == 2)
                    {
                        EnablePercentageDiscount = false;
                        EnableFixedDiscount = true;
                    }
                    IsDiscountGiven = true;
                    OnPropertyChanged();
                    if (_selectedDiscountType != 0)
                        CalculateTotal();
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
        public ViewOrderViewModel(DatabaseService databaseService, MenuService menuService, OrdersViewModel ordersViewModel, SettingService settingService)
        {
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _settingService = settingService;
            _menuService = menuService;
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
            IsLoading = true;

            OrderItems.Clear();

            Categories = await _menuService.GetMenuCategories();

            Categories[0].IsSelected = true;
            SelectedCategory = Categories[0];

            MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

            await GetOrderDetailsAsync();
            await GetOrderKOTsAsync();

            IsLoading = false;
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        private async Task GetOrderDetailsAsync()
        {
            var order = await _databaseService.GetOrderById(TableModel.RunningOrderId);

            OrderModel = new OrderModel
            {
                Id = order.Id,
                TableId = order.TableId,
                OrderDate = order.OrderDate,
                TotalItemCount = order.TotalItemCount,
                TotalPrice = order.TotalPrice,
                PaymentMode = order.PaymentMode,
                OrderStatus = order.OrderStatus,
            };
        }

        /// <summary>
        /// Command to get the items of selected order
        /// </summary>
        /// <returns>Returns Task Object</returns>
        private async Task GetOrderKOTsAsync()
        {
            IsLoading = true;

            OrderKOTs = (await _databaseService.GetOrderKotsAsync(OrderModel.Id))
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

        /// <summary>
        /// Change data as per selected category
        /// Working as a relay command
        /// </summary>
        /// <param name="categoryId">CategoryId of the MenuCategory selected</param>
        [RelayCommand]
        private async Task SelectCategoryAsync(int categoryId)
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
        private async Task DiscountPercentageClickAsync()
        {
            var result = await Shell.Current.DisplayPromptAsync("Discount Percentage", "Enter the applicable discount percentage.", placeholder: "10", initialValue: DiscountPercentage.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {

                if (!Decimal.TryParse(result, out decimal enteredDiscountPercentage))
                {
                    await Shell.Current.DisplayAlert("Invalid Value", "Entered discount percentage is invalid.", "Ok");
                    return;
                }

                DiscountPercentage = enteredDiscountPercentage;
                Total = SubTotal - DiscountPercentageAmount;
            }
            IsUpdated = true;
        }

        /// <summary>
        /// Command to open a dialog box for accepting tax percentage
        /// </summary>
        /// <returns>A Task object</returns>
        [RelayCommand]
        private async Task DiscountFixedClickAsync()
        {
            var result = await Shell.Current.DisplayPromptAsync("Discount Fixed", "Enter the applicable discount amount.", placeholder: "10", initialValue: DiscountFixed.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {

                if (!Decimal.TryParse(result, out decimal enteredFixedDiscount))
                {
                    await Shell.Current.DisplayAlert("Invalid Value", "Entered discount amount is invalid.", "Ok");
                    return;
                }

                DiscountFixed = enteredFixedDiscount;
                Total = SubTotal - DiscountFixed;
            }
            IsUpdated = true;
        }

        /// <summary>
        /// Command to be called when search box changes
        /// </summary>
        /// <param name="textSearch">Query to search</param>
        [RelayCommand]
        private void SearchItems(string? textSearch)
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
                    IsPercentageBased = EnablePercentageDiscount,
                    DiscountFixed = DiscountFixed,
                    DiscountPercentage = DiscountPercentage,
                };
                await _databaseService.DiscountOperations.SaveDiscountAsync(discount);
            }

            TableModel.Status = TableOrderStatus.Confirmed;
            // Push for change in table info
            WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        /// <summary>
        /// Command to cancel given discount
        /// </summary>
        [RelayCommand]
        private void CancelDiscount()
        {
            IsDiscountGiven = false;
            EnableFixedDiscount = false;
            EnablePercentageDiscount = false;
        }

        /// <summary>
        /// To calculate the total amount
        /// </summary>
        private void CalculateTotal()
        {
            Total = SubTotal;
            if (SelectedDiscountType == 1)
            {
                Total = SubTotal - DiscountPercentageAmount;
            }
            else if (SelectedDiscountType == 2)
            {
                Total = SubTotal - DiscountFixed;
            }
        }
    }
}
