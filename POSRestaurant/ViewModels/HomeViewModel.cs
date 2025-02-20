using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LoggerService;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using SettingLibrary;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Home Page
    /// </summary>
    public partial class HomeViewModel : ObservableObject, IRecipient<MenuItemChangedMessage>, IRecipient<StaffChangedMessage>
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        private readonly LogService _logger;

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
        /// To track the order type on UI
        /// Made observable for using in UI
        /// </summary>
        [ObservableProperty]
        private OrderTypes _orderType = OrderTypes.DineIn;

        /// <summary>
        /// To enable or disable the OrderType selection
        /// </summary>
        [ObservableProperty]
        private bool _orderTypeEnable = true;
        
        /// <summary>
        /// To enable or disable the OrderType selection
        /// </summary>
        [ObservableProperty]
        private bool _numberOfPeopleEnable = true;

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
        private readonly SettingService _settingService;

        /// <summary>
        /// ObservableProperty for the SearchBox, to search for items
        /// </summary>
        [ObservableProperty]
        private string _textSearch;

        /// <summary>
        /// To keep track of the number of peopl sitting on a table
        /// </summary>
        [ObservableProperty]
        public int _numberOfPeople = 1;

        /// <summary>
        /// To manage the selected order type on main page
        /// </summary>
        private int _selectedOrderType;

        /// <summary>
        /// To manage the selected order type on main page
        /// Should be handled by code as well
        /// </summary>
        public int SelectedOrderType
        {
            get => _selectedOrderType;
            set
            {
                if (_selectedOrderType != value)
                {
                    _selectedOrderType = value;
                    switch (Convert.ToInt32(_selectedOrderType))
                    {
                        case (int)OrderTypes.DineIn:
                            OrderType = OrderTypes.DineIn;
                            break;
                        case (int)OrderTypes.Pickup:
                            OrderType = OrderTypes.Pickup;
                            break;
                    }
                    OnOrderTypeChanged();
                }
            }
        }

        /// <summary>
        /// List of waiters to be assigned to the order
        /// </summary>
        [ObservableProperty]
        public StaffModel[] _waiters;

        /// <summary>
        /// To manage the selected waiter for the order
        /// </summary>
        private StaffModel _selectedWaiter;

        /// <summary>
        /// To manage the selection of waiter on main page
        /// Should be assigned by code as well
        /// </summary>
        public StaffModel SelectedWaiter
        {
            get => _selectedWaiter;
            set
            {
                if (_selectedWaiter != value)
                {
                    _selectedWaiter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// To handle the property changed event for the radio button switch
        /// </summary>
        public event PropertyChangedEventHandler OrderTypePropertyChanged;

        /// <summary>
        /// Called when OrderType is changed
        /// </summary>
        /// <param name="orderType">Selected OrderType name</param>
        protected virtual void OnOrderTypeChanged([CallerMemberName] string orderType = null)
        {
            OrderTypePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(orderType));
        }

        /// <summary>
        /// To handle the property changed event for the waiter selected
        /// </summary>
        public event PropertyChangedEventHandler WaiterPropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string waiter = null)
        {
            WaiterPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(waiter));
        }

        /// <summary>
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public HomeViewModel(LogService logger, DatabaseService databaseService, MenuService menuService, OrdersViewModel ordersViewModel, SettingService settingService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _menuService = menuService;

            _ordersViewModel = ordersViewModel;
            _settingService = settingService;
            CartItems.CollectionChanged += CartItems_CollectionChanged;

            Name = _settingService.Settings.CustomerName;

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<MenuItemChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<StaffChangedMessage>(this);

            // Set the initial selection
            SelectedOrderType = 1; // Default to "Dine In"
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync(TableModel tableModel)
        {
            if (tableModel.Status != TableOrderStatus.NoOrder)
            {
                OrderTypeEnable = false;
                NumberOfPeopleEnable = false;
                NumberOfPeople = tableModel.NumberOfPeople;
                SelectedOrderType = (int)tableModel.OrderType;
                SelectedWaiter = tableModel.Waiter;
            }
            else
            {
                OrderTypeEnable = true;
                NumberOfPeopleEnable = true;
                NumberOfPeople = 1;
                SelectedOrderType = 1;
            }

            if (_isInitialized)
            {
                foreach(var category in Categories)
                {
                    category.IsSelected = false;
                }
                Categories[0].IsSelected = true;
                SelectedCategory = Categories[0];

                return;
            }

            _isInitialized = true;

            IsLoading = true;

            Categories = await _menuService.GetMenuCategories();

            Categories[0].IsSelected = true;
            SelectedCategory = Categories[0];

            MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

            await LoadWaiters();

            CartItems.Clear();

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
            SubTotal = CartItems.Sum(o => o.Amount);
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
        private async Task PlaceOrderAsync(TableModel tableModel)
        {
            if (NumberOfPeople == 0)
            {
                await Shell.Current.DisplayAlert("Order Error", "Number of people should be greater than 0.", "Ok");
                return;
            }

            if (SelectedWaiter == null)
            {
                await Shell.Current.DisplayAlert("Order Error", "Assign a waiter to the order.", "Ok");
                return;
            }


            IsLoading = true;

            if (tableModel.RunningOrderId == 0)
            {
                tableModel.OrderType = OrderType;
                tableModel.Waiter = SelectedWaiter;
                tableModel.NumberOfPeople = NumberOfPeople;
            }

            if (await _ordersViewModel.PlaceKOTAsync([.. CartItems], tableModel, OrderType, SelectedWaiter))
            {
                CartItems.Clear();

                // Push for change in table info
                WeakReferenceMessenger.Default.Send(TableChangedMessage.From(tableModel));
            }

            IsLoading = false;

            await Application.Current.MainPage.Navigation.PopAsync();
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
        /// Implemented interface IRecipient
        /// </summary>
        /// <param name="message">ItemOnMenuModel published from other parts of the application</param>
        public void Receive(MenuItemChangedMessage message)
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

        /// <summary>
        /// Command to increase number of people
        /// </summary>
        [RelayCommand]
        private void IncreaseNumberOfPeople()
        {
            NumberOfPeople += 1;
        }

        /// <summary>
        /// Command to decrease number of people
        /// </summary>
        [RelayCommand]
        private void DecreaseNumberOfPeople()
        {
            if (NumberOfPeople - 1 < 1)
                return;
            
            NumberOfPeople -= 1;
        }

        /// <summary>
        /// Refresh staff details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(StaffChangedMessage message)
        {
            await LoadWaiters();
        }

        /// <summary>
        /// To call the database and load the list of waiters
        /// </summary>
        /// <returns>Returns a task object</returns>
        private async Task LoadWaiters()
        {
            Waiters = (await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.Waiter))
                            .Select(StaffModel.FromEntity)
                            .ToArray();
        }
    }
}
