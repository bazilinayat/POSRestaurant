using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.Models;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Home Page
    /// </summary>
    public partial class HomeViewModel : ObservableObject
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
        /// ObservableCollection for items added to cart
        /// </summary>
        public ObservableCollection<CartItemModel> CartItems { get; set; } = new();

        /// <summary>
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public HomeViewModel(DatabaseService databaseService)
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
            if (_isInitialized)
                return;

            _isInitialized = true;

            IsLoading = true;

            Categories = (await _databaseService.GetMenuCategoriesAsync())
                            .Select(MenuCategoryModel.FromEntity)
                            .ToArray();

            Categories[0].IsSelected = true;
            SelectedCategory = Categories[0];

            MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

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

            MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

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
            }
        }

        /// <summary>
        /// Command to increase item quantity
        /// </summary>
        /// <param name="cartItem">Item from cart to increase quantity</param>
        [RelayCommand]
        private void IncreaseQuantity(CartItemModel cartItem) =>
            cartItem.Quantity++;

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
        }

        /// <summary>
        /// Command to remove item from cart
        /// </summary>
        /// <param name="cartItem">Item from cart to remove</param>
        [RelayCommand]
        private void RemoveItemFromCart(CartItemModel cartItem) =>
            CartItems.Remove(cartItem);
    }
}
