using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Class to connect and talk to our database
    /// </summary>
    public class DatabaseService : IAsyncDisposable
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Class constructor, to generate database and connection
        /// </summary>
        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RestPOS.db3");
            _connection = new SQLiteAsyncConnection(dbPath, 
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        }

        /// <summary>
        /// Method to initialize database - create necessary tables.
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async Task InitializeDatabaseAsync()
        {
            await _connection.CreateTableAsync<MenuCategory>();
            await _connection.CreateTableAsync<ItemOnMenu>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<OrderItem>();

            await SeedDataAsync();
        }

        /// <summary>
        /// Method to load our seed data, taken from SeedData class
        /// </summary>
        /// <returns>Returns a Task object</returns>
        private async Task SeedDataAsync()
        {
            // Checking and return if data is already seeded
            var firstCategory = await _connection.Table<MenuCategory>().FirstOrDefaultAsync();
            if (firstCategory != null)
                return;

            var categories = SeedData.GetMenuCategories();
            var menuItems = SeedData.GetMenuItems();

            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItems);
        }

        /// <summary>
        /// Method to get all the categories from database
        /// </summary>
        /// <returns></returns>
        public async Task<MenuCategory[]> GetMenuCategoriesAsync() =>
            await _connection.Table<MenuCategory>().ToArrayAsync();

        public async Task<ItemOnMenu[]> GetMenuItemsAsync(int categoryId) =>
            await _connection.Table<ItemOnMenu>().Where(o => o.MenuCategoryId == categoryId).ToArrayAsync();

        /// <summary>
        /// Implementation of IAsyncDisposable interface
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
