using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;

namespace POSRestaurant.Service
{
    /// <summary>
    /// To handle all the business logic and db operations for menu
    /// To use as a singleton instance throughout application
    /// To load menu in code once only
    /// </summary>
    public class MenuService
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To check only one instance is available
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// To store the menu categories in memory
        /// </summary>
        private MenuCategoryModel[] _menuCategories;

        /// <summary>
        /// To store the menu items for each category in memory
        /// </summary>
        private Dictionary<int, ItemOnMenu[]> _menuItems;


        public MenuService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// To get the MenuCategories from the DB
        /// </summary>
        /// <returns>Array of MenuCategory</returns>
        public async Task<MenuCategoryModel[]> GetMenuCategories()
        {
            if (_menuCategories == null)
            {
                await _semaphore.WaitAsync();
                try
                {
                    // Check again inside the semaphore in case another thread already populated _menuItems
                    if (_menuCategories == null)
                    {
                        _menuCategories = await LoadMenuCategories();
                    }
                }
                finally
                {
                    _semaphore.Release(); // End critical section.
                }
            }
            return _menuCategories;
        }

        /// <summary>
        /// To get the MenuCategories from the DB
        /// </summary>
        /// <returns>Array of MenuCategory</returns>
        public async Task<ItemOnMenu[]> GetCategoryItems(int categoryId)
        {
            if (_menuItems == null)
            {
                await _semaphore.WaitAsync();
                try
                {
                    // Check again inside the semaphore in case another thread already populated _menuItems
                    if (_menuItems == null)
                    {
                        _menuItems = await LoadCategoryItems();
                    }
                }
                finally
                {
                    _semaphore.Release(); // End critical section.
                }
            }
            return _menuItems[categoryId];
        }

        /// <summary>
        /// To use the database service and load the menu categories in memory
        /// </summary>
        public async Task<MenuCategoryModel[]> LoadMenuCategories()
        {
            return (await _databaseService.MenuOperations.GetMenuCategoriesAsync())
                .Select(MenuCategoryModel.FromEntity)
                .ToArray();
        }

        /// <summary>
        /// To use the database service and load the menu category items in memory
        /// </summary>
        public async Task<Dictionary<int, ItemOnMenu[]>> LoadCategoryItems()
        {
            _menuItems = new Dictionary<int, ItemOnMenu[]>();

            foreach (var menuCategory in _menuCategories)
            {
                _menuItems.Add(menuCategory.Id, await _databaseService.MenuOperations.GetMenuItemsByCategoryAsync(menuCategory.Id));
            }

            return _menuItems;
        }
    }
}
