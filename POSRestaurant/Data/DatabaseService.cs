using POSRestaurant.Models;
using SQLite;

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
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RestPOS.db3");
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

        public async Task<ItemOnMenu[]> GetMenuItemsByCategoryAsync(int categoryId) =>
            await _connection.Table<ItemOnMenu>().Where(o => o.MenuCategoryId == categoryId).ToArrayAsync();

        /// <summary>
        /// Method to place order, add order, order items and revert if anything goes wrong
        /// </summary>
        /// <param name="orderModel">Order to be placed, with items</param>
        /// <returns>Returns Error Message or null (in case of success)</returns>
        public async Task<string?> PlaceOrderAsync(OrderModel orderModel)
        {
            var order = new Order()
            {
                OrderDate = orderModel.OrderDate,
                PaymentMode = orderModel.PaymentMode,
                TotalItemCount = orderModel.TotalItemCount,
                TotalPrice = orderModel.TotalPrice
            };

            if (await _connection.InsertAsync(order) > 0)
            {
                // Order inserted successfully, get order Id and add order items
                foreach (var item in orderModel.Items)
                {
                    item.OrderId = order.Id;
                }
                if (await _connection.InsertAllAsync(orderModel.Items) == 0)
                {
                    // Order items ooperation failed
                    // Remove the associated order
                    await _connection.DeleteAsync(order);
                    return "Error in inserting order items";
                }
            }
            else
            {
                return "Error in inserting the order";
            }
            orderModel.Id = order.Id;
            return null;
        }

        /// <summary>
        /// Get a array of all the orders, if needed, you can apply paging here
        /// </summary>
        /// <returns>Array of orders</returns>
        public async Task<Order[]> GetOrdersAsync() =>
            await _connection.Table<Order>().ToArrayAsync();

        /// <summary>
        /// Method to get all the items from a specific order
        /// </summary>
        /// <param name="orderId">For which order</param>
        /// <returns>Array of items in the order</returns>
        public async Task<OrderItem[]> GetOrderItemsAsync(long orderId) =>
            await _connection.Table<OrderItem>().Where(o => o.OrderId == orderId).ToArrayAsync();

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
