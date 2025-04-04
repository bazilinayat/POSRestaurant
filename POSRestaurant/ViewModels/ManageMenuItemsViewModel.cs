﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;

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
        /// DIed LogService
        /// </summary>
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
        /// DIed SettingService
        /// </summary>
        private readonly Setting _settingService;

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
        public ManageMenuItemViewModel(LogService logger, DatabaseService databaseService, MenuService menuService, Setting settingService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _menuService = menuService;
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
            await GetSetRequiredValues();
        }

        /// <summary>
        /// To reset the screen for this page
        /// </summary>
        /// <returns></returns>
        private async Task GetSetRequiredValues()
        {
            try
            {
                IsLoading = true;

                Categories = await _menuService.GetMenuCategories();

                if (!_isInitialized)
                {
                    Categories[0].IsSelected = true;
                    SelectedCategory = Categories[0];
                }
                else
                {
                    var category = Categories.FirstOrDefault(o => o.Id == SelectedCategory.Id);
                    category.IsSelected = true;
                }

                MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);

                AddNewItemMenuItem();

                _isInitialized = true;
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-GetSetRequiredValues Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Manage Menu Screen", "OK");
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

                AddNewItemMenuItem();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-SelectCategoryAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Category", "OK");
            }
        }

        /// <summary>
        /// When we select a menu item to edit
        /// </summary>
        /// <param name="itemOnMenu">MenuItem to be edited</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task EditMenuItemAsync(ItemOnMenu itemOnMenu)
        {
            try
            {
                var menuItemModel = new ItemOnMenuModel
                {
                    Description = itemOnMenu.Description,
                    Name = itemOnMenu.Name,
                    Price = itemOnMenu.Price,
                    Id = itemOnMenu.Id,
                    ShortCode = itemOnMenu.ShortCode
                };

                if (itemOnMenu.Id != 0)
                {
                    var itemCategory = await _databaseService.MenuOperations.GetCategoryOfMenuItem(itemOnMenu.Id);

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
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-EditMenuItemAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Item", "OK");
            }
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
            try
            {
                IsLoading = true;

                var errorMessage = await _databaseService.MenuOperations.SaveMenuItemAsync(item);

                if (errorMessage != null)
                {
                    await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                    IsLoading = false;
                    return;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Successful", "Item save successfully", "OK");

                    await GetSetRequiredValues();

                    /* Sending the new item, changed item to WeakReferenceMessager
                     * This is the publishing part
                     */
                    WeakReferenceMessenger.Default.Send(MenuItemChangedMessage.From(new ItemOnMenuChangeModel
                    {
                        IsDeleted = false,
                        ItemModel = item
                    }));

                    Cancel();
                }
                await _menuService.LoadCategoryItems();
                MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);
                AddNewItemMenuItem();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-SaveItem Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving Menu Item", "OK");
            }
        }

        /// <summary>
        /// To delete the item sent by the control
        /// </summary>
        /// <param name="item">MenuItem to delete</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItem(ItemOnMenuModel item)
        {
            try
            {
                var menuItem = new ItemOnMenu
                {
                    Id = item.Id,
                    Description = item.Description,
                    Name = item.Name,
                    Price = item.Price,
                    MenuCategoryId = item.Category.Id
                };
                if (await _databaseService.MenuOperations.DeleteMenuItemAsync(menuItem) > 0)
                {
                    await Shell.Current.DisplayAlert("Successful", $"{item.Name} deleted successfully", "OK");

                    await GetSetRequiredValues();

                    /* Sending the new item, changed item to WeakReferenceMessager
                     * This is the publishing part
                     */
                    WeakReferenceMessenger.Default.Send(MenuItemChangedMessage.From(new ItemOnMenuChangeModel
                    {
                        IsDeleted = true,
                        ItemModel = item
                    }));
                    await _menuService.LoadCategoryItems();
                    MenuItems = await _menuService.GetCategoryItems(SelectedCategory.Id);
                    AddNewItemMenuItem();

                    Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-DeleteItem Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Deleting Menu Item", "OK");
            }
        }

        /// <summary>
        /// To add a new option in menu items for add item
        /// </summary>
        private void AddNewItemMenuItem()
        {
            try
            {
                var newItem = new ItemOnMenu
                {
                    Name = "Add New Item"
                };
                var newArray = new ItemOnMenu[MenuItems.Length + 1];
                newArray[0] = newItem;
                Array.Copy(MenuItems, 0, newArray, 1, MenuItems.Length);
                MenuItems = newArray;
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageMenuItemsVM-AddNewItemMenuItem Error", ex);
            }
        }
    }
}
