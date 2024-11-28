using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.Models;
using POSRestaurant.Utility;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// View Model for ManageMenuItems page
    /// </summary>
    public partial class ManageMenuItemViewModel : ObservableObject
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
        /// Property to observe the selected item on UI
        /// </summary>
        [ObservableProperty]
        private ItemOnMenuModel _menuItem = new();

        /// <summary>
        /// Constructor for ManageMenuItemsViewModel
        /// </summary>
        /// <param name="databaseService">DIed DatabaseService</param>
        /// <param name="settingService">DIed SettingService</param>
        public ManageMenuItemViewModel(DatabaseService databaseService, SettingService settingService)
        {
            _databaseService = databaseService;
            _settingService = settingService;
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

            var newItem = new ItemOnMenu
            {
                Name = "Add New Item"
            };
            var newArray = new ItemOnMenu[MenuItems.Length + 1];
            newArray[0] = newItem;
            Array.Copy(MenuItems, 0, newArray, 1, MenuItems.Length);
            MenuItems = newArray;

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

            var newItem = new ItemOnMenu
            {
                Name = "Add New Item"
            };
            var newArray = new ItemOnMenu[MenuItems.Length + 1];
            newArray[0] = newItem;
            Array.Copy(MenuItems, 0, newArray, 1, MenuItems.Length);
            MenuItems = newArray;

            IsLoading = false;
        }

        /// <summary>
        /// When we select a menu item to edit
        /// </summary>
        /// <param name="itemOnMenu">MenuItem to be edited</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task EditMenuItemAsync(ItemOnMenu itemOnMenu)
        {
            var menuItemModel = new ItemOnMenuModel
            {
                Description = itemOnMenu.Description,
                Name = itemOnMenu.Name,
                Price = itemOnMenu.Price,
                Id = itemOnMenu.Id,
            };

            if (itemOnMenu.Id != 0)
            {
                var itemCategory = await _databaseService.GetCategoryOfMenuItem(itemOnMenu.Id);

                var categorOfItem = new MenuCategoryModel
                {
                    Icon = itemCategory.Icon,
                    Name = itemCategory.Name,
                    Id = itemCategory.Id,
                };
                menuItemModel.Category = categorOfItem; 
            }
            else
            {
                menuItemModel.Category = SelectedCategory;
            }

            MenuItem = menuItemModel;
        }

        /// <summary>
        /// To handle cancel button click on the control
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            MenuItem = new();
        }

        /// <summary>
        /// To save or update the item sent by the control
        /// </summary>
        /// <param name="item">MenuItem to save or update</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task SaveItem(ItemOnMenuModel item)
        {
            IsLoading = true;

            var errorMessage = await _databaseService.SaveMenuItemAsync(item);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Successful", "Item save successfully", "OK");
            }

            IsLoading = false;
        }

        /// <summary>
        /// To delete the item sent by the control
        /// </summary>
        /// <param name="item">MenuItem to delete</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItem(ItemOnMenuModel item)
        {
            var menuItem = new ItemOnMenu
            {
                Id = item.Id,
                Description = item.Description,
                Name = item.Name,
                Price = item.Price,
                MenuCategoryId = item.Category.Id
            };
            if (await _databaseService.DeleteMenuItemAsync(menuItem) > 0)
            {
                await Shell.Current.DisplayAlert("Successful", $"{item.Name} deleted successfully", "OK");
            }
        }
    }
}
