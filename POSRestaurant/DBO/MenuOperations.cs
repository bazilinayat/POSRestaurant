using POSRestaurant.Data;
using POSRestaurant.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle all the database menu operations
    /// </summary>
    public class MenuOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public MenuOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Method to get all the categories from database
        /// </summary>
        /// <returns>Array of MenuCategory</returns>
        public async Task<MenuCategory[]> GetMenuCategoriesAsync() =>
            await _connection.Table<MenuCategory>().ToArrayAsync();

        /// <summary>
        /// Method to get all the MenuItems under the selected categoryId
        /// </summary>
        /// <param name="categoryId">Selected category id</param>
        /// <returns>Array of ItemOnMenu</returns>
        public async Task<ItemOnMenu[]> GetMenuItemsByCategoryAsync(int categoryId) =>
            await _connection.Table<ItemOnMenu>().Where(o => o.MenuCategoryId == categoryId).ToArrayAsync();

        /// <summary>
        /// To get the categorie(s) the menu item belongs to
        /// </summary>
        /// <param name="menuItemId">Item Id</param>
        /// <returns>MenuCategory Array</returns>
        public async Task<MenuCategory> GetCategoryOfMenuItem(int menuItemId)
        {
            var itemOnMenu = await _connection.Table<ItemOnMenu>().Where(o => o.Id == menuItemId).FirstOrDefaultAsync();

            return await _connection.Table<MenuCategory>().Where(o => o.Id == itemOnMenu.MenuCategoryId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Delete MenuItem from the database
        /// </summary>
        /// <param name="item">Item to delete</param>
        /// <returns>Returns the number of rows deleted</returns>
        public async Task<int> DeleteMenuItemAsync(ItemOnMenu item)
        {
            return await _connection.DeleteAsync(item);
        }

        /// <summary>
        /// To save the menu item in database
        /// </summary>
        /// <param name="itemModel">Item to be saved</param>
        /// <returns>Error Message String</returns>
        public async Task<string?> SaveMenuItemAsync(ItemOnMenuModel itemModel)
        {
            ItemOnMenu menuItem = new ItemOnMenu
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Description = itemModel.Description,
                MenuCategoryId = itemModel.Category.Id,
                Price = itemModel.Price,
            };

            if (menuItem.Id == 0)
            {
                if (await _connection.InsertAsync(menuItem) > 0)
                    return null;

                return "Error in saving menu item";
            }
            else
            {
                if (await _connection.UpdateAsync(menuItem) > 0)
                    return null;

                return "Error in updating menu item";
            }
        }
    }
}
