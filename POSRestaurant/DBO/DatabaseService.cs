﻿using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Utility;
using SQLite;
using System.Linq;

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
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// Variable for getting initial data
        /// </summary>
        private readonly SeedData _seedData;

        /// <summary>
        /// To handle calls to staff operations in the db
        /// </summary>
        public StaffOperations StaffOperaiotns;

        /// <summary>
        /// Class constructor, to generate database and connection
        /// </summary>
        /// <param name="settingService">DI for SettingService</param>
        public DatabaseService(SettingService settingService)
        {
            _settingService = settingService;
            _seedData = new SeedData(_settingService);
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data",  "RestPOS.db3");
            _connection = new SQLiteAsyncConnection(dbPath, 
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

            StaffOperaiotns = new StaffOperations(_connection);
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

            await _connection.CreateTableAsync<Table>();
            await _connection.CreateTableAsync<KOT>();
            await _connection.CreateTableAsync<KOTItem>();

            await _connection.CreateTableAsync<Staff>();

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

            var categories = _seedData.GetMenuCategories();
            var menuItems = _seedData.GetMenuItems();
            var tables = _seedData.GetTables();

            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItems);
            await _connection.InsertAllAsync(tables);
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
        /// Method to place order, add order, order items and revert if anything goes wrong
        /// </summary>
        /// <param name="orderModel">Order to be placed, with items</param>
        /// <returns>Returns Error Message or null (in case of success)</returns>
        public async Task<string?> PlaceOrderAsync(OrderModel orderModel)
        {
            var order = new Order()
            {
                TableId = orderModel.TableId,
                OrderDate = orderModel.OrderDate,
                PaymentMode = orderModel.PaymentMode,
                TotalItemCount = orderModel.TotalItemCount,
                TotalPrice = orderModel.TotalPrice,
                OrderStatus = orderModel.OrderStatus,
                OrderNumber = orderModel.OrderNumber,
                OrderType = orderModel.OrderType,
            };

            if (await _connection.InsertAsync(order) > 0)
            {
                // Order inserted successfully, add kots
                foreach (var kotItem in orderModel.KOTs)
                {
                    var kot = new KOT()
                    {
                        OrderId = order.Id,
                        KOTNumber = 1,
                        KOTDateTime = kotItem.KOTDateTime,
                        TotalItemCount = kotItem.TotalItemCount,
                        TotalPrice = kotItem.TotalPrice
                    };

                    if (await _connection.InsertAsync(kot) > 0)
                    {
                        foreach (var item in kotItem.Items)
                        {
                            item.KOTId = kot.Id;
                        }
                        if (await _connection.InsertAllAsync(kotItem.Items) == 0)
                        {
                            // Order items ooperation failed
                            // Remove the associated order
                            await DeleteOrderAsync(order);
                            return "Error in inserting order items";
                        }
                    }
                }
                orderModel.Id = order.Id;
                return null;
            }
            else
            {
                return "Error in inserting the order";
            }
        }

        /// <summary>
        /// To insert the kots in an order
        /// </summary>
        /// <param name="KOTs">KOTs to be added</param>
        /// <param name="runningOrderId">Running order Id for the selected table, for which KOT to be added</param>
        /// <returns>Returns Error Message or null(in case of success)</returns>
        public async Task<string?> InsertOrderKOTAsync(KOTModel[] KOTs, long runningOrderId)
        {
            var orderModel = await GetOrderById(runningOrderId);

            // First we add the kot for the order
            foreach (var kotItem in KOTs)
            {
                var kot = new KOT()
                {
                    OrderId = runningOrderId,
                    KOTNumber = kotItem.KOTNumber,
                    KOTDateTime = kotItem.KOTDateTime,
                    TotalItemCount = kotItem.TotalItemCount,
                    TotalPrice = kotItem.TotalPrice
                };

                if (await _connection.InsertAsync(kot) > 0)
                {
                    foreach (var item in kotItem.Items)
                    {
                        item.KOTId = kot.Id;
                    }
                    if (await _connection.InsertAllAsync(kotItem.Items) == 0)
                    {
                        return "Error in inserting order items";
                    }

                    // Update the order details, for each kot
                    orderModel.TotalPrice += kotItem.TotalPrice;
                    orderModel.TotalItemCount += kotItem.TotalItemCount;
                    orderModel.OrderDate = DateTime.Now;
                }
            }

            // Update the order object with latest details
            await _connection.UpdateAsync(orderModel);

            return null;
        }

        /// <summary>
        /// Delete order based on given OrderId
        /// </summary>
        /// <param name="order">Order to delete</param>
        /// <returns>Returns a Task object</returns>
        public async Task DeleteOrderAsync(Order order) =>
            await _connection.DeleteAsync(order);

        /// <summary>
        /// To get the last order id
        /// Later on can be modified for table id as well.
        /// </summary>
        /// <returns>The latest order id</returns>
        public async Task<Order> GetLatestOrderId() =>
            await _connection.Table<Order>().Where(o => o.OrderStatus == TableOrderStatus.Running).OrderByDescending(o => o.OrderDate).FirstOrDefaultAsync();

        /// <summary>
        /// To get the order as per the given order id
        /// </summary>
        /// <param name="orderId">order id for the selected table</param>
        /// <returns>Returns a Order Object</returns>
        public async Task<Order> GetOrderById(long orderId) =>
            await _connection.Table<Order>().FirstOrDefaultAsync(o => o.Id == orderId);

        /// <summary>
        /// To get the last order id for given table id
        /// Later on can be modified for table id as well.
        /// </summary>
        /// <returns>The latest order id</returns>
        public async Task<Order> GetLatestOrderIdByTableId(int tableId) =>
            await _connection.Table<Order>().Where(o => o.OrderStatus == TableOrderStatus.Running && o.TableId == tableId).OrderByDescending(o => o.OrderDate).FirstOrDefaultAsync();

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
        /// Method to get all the kots from a specific order
        /// </summary>
        /// <param name="orderId">For which order</param>
        /// <returns>Array of items in the order</returns>
        public async Task<KOT[]> GetOrderKotsAsync(long orderId)
        {
            var kots = await _connection.Table<KOT>().Where(o => o.OrderId == orderId).ToArrayAsync();
            foreach(var kot in kots)
            {
                kot.Items = await GetKotItemsAsync(kot.Id);
            }
            return kots;
        }

        /// <summary>
        /// Method to get all the items from a specific Kot
        /// </summary>
        /// <param name="KotId">For which Kot</param>
        /// <returns>Array of items in the Kot</returns>
        public async Task<KOTItem[]> GetKotItemsAsync(long KotId) =>
            await _connection.Table<KOTItem>().Where(o => o.KOTId == KotId).ToArrayAsync();        

        /// <summary>
        /// To Get menu item by name thorugh searching in database
        /// </summary>
        /// <param name="searchText">The item name to search</param>
        /// <returns>Returns a array of ItemOnMenu</returns>
        public async Task<ItemOnMenu[]> GetMenuItemBySearch(string searchText) =>
            await _connection.Table<ItemOnMenu>().Where(o => o.ShortCode.Contains(searchText)).ToArrayAsync();

        /// <summary>
        /// Method to get all the tables from database
        /// </summary>
        /// <returns>Array of Table</returns>
        public async Task<Table[]> GetTablesAsync() =>
            await _connection.Table<Table>().ToArrayAsync();

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

        /// <summary>
        /// To get the last KOT number for orderId, so we can number the new kots properly
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns>Returns the last KOT Number</returns>
        public async Task<int> GetLastKOTNumberForOrderId(long orderId)
        {
            var kot = await _connection.Table<KOT>().Where(o => o.OrderId == orderId).OrderByDescending(o => o.KOTNumber).FirstOrDefaultAsync();
            if (kot == null)
                return 0;
            else
                return kot.KOTNumber;
        }

        /// <summary>
        /// To get the KOT from database
        /// </summary>
        /// <param name="KOTId">KOTId of the KOT to get</param>
        /// <returns>KOT Object</returns>
        public async Task<KOT> GetKOTAsync(long KOTId) =>
            await _connection.Table<KOT>().FirstOrDefaultAsync(o => o.Id == KOTId);

        /// <summary>
        /// To delete the kotitems and update there corresponding KOTs as well
        /// </summary>
        /// <param name="recordsToDelete">Dictionary of KOTid and KOTItems to delete and Update</param>
        /// <param name="runningOrderId">Order to update for KOTs</param>
        /// <returns>Error Message or null on success</returns>
        public async Task<string?> DeleteKOTItemsAndUpdateKOT(Dictionary<long, KOTItem[]> recordsToDelete, long runningOrderId)
        {
            string errorMesasge = null;

            await _connection.RunInTransactionAsync(async (db) =>
            {
                foreach (var record in recordsToDelete)
                {
                    int deleted = 0;
                    foreach (var item in record.Value)
                    {
                        deleted += db.Delete(item);
                    }
                    if (deleted > 0)
                    {
                        var kotToUpdate =  await GetKOTAsync(record.Key);

                        var kotItems = await GetKotItemsAsync(record.Key);

                        if (kotItems.Length == 0)
                        {
                            db.Delete(kotToUpdate);
                        }
                        else
                        {
                            kotToUpdate.TotalItemCount = 0;
                            kotToUpdate.TotalPrice = 0;
                            for (int i = 0; i < kotItems.Length; i++)
                            {
                                kotToUpdate.TotalItemCount += kotItems[i].Quantity;
                                kotToUpdate.TotalPrice += kotItems[i].Amount;
                            }

                            if (db.Update(kotToUpdate) == 0)
                            {
                                errorMesasge = "Error in updating Order";
                                throw new Exception("Error update KOT");
                            }
                        }

                        var orderModel = await GetOrderById(runningOrderId);
                        orderModel.TotalPrice = 0;
                        orderModel.TotalItemCount = 0;
                        orderModel.OrderDate = DateTime.Now;

                        var kots = await GetOrderKotsAsync(runningOrderId);

                        for (int i = 0; i < kots.Length; i++)
                        {
                            orderModel.TotalPrice += kots[i].TotalPrice;
                            orderModel.TotalItemCount += kots[i].TotalItemCount;
                        }

                        if (db.Update(orderModel) == 0)
                        {
                            errorMesasge = "Error in updating Order";
                            throw new Exception("Error update Order");
                        } 
                    }
                }
            });

            return errorMesasge;
        }

        /// <summary>
        /// To update the kotitems and update there corresponding KOTs as well
        /// </summary>
        /// <param name="recordsToUpdate">Dictionary of KOTid and KOTItems to update</param>
        /// <param name="runningOrderId">Order to update for KOTs</param>
        /// <returns>Error Message or null on success</returns>
        public async Task<string?> UpdateKOTItemsAndKOT(Dictionary<long, KOTItem[]> recordsToUpdate, long runningOrderId)
        {
            string errorMesasge = null;

            await _connection.RunInTransactionAsync(async(db) =>
            {
                foreach (var record in recordsToUpdate)
                {
                    int updated = 0;
                    foreach (var item in record.Value)
                    {
                        updated += db.Update(item);
                    }
                    if (updated > 0)
                    {
                        var kotToUpdate = await GetKOTAsync(record.Key);
                        kotToUpdate.TotalItemCount = 0;
                        kotToUpdate.TotalPrice = 0;

                        var kotItems = await GetKotItemsAsync(record.Key);

                        for (int i = 0; i < kotItems.Length; i++)
                        {
                            kotToUpdate.TotalItemCount += kotItems[i].Quantity;
                            kotToUpdate.TotalPrice += kotItems[i].Amount;
                        }

                        if (db.Update(kotToUpdate) == 0)
                        {
                            errorMesasge = "Error in updating Order";
                            throw new Exception("Error update KOT");
                        }

                        var orderModel = await GetOrderById(runningOrderId);
                        orderModel.TotalPrice = 0;
                        orderModel.TotalItemCount = 0;
                        orderModel.OrderDate = DateTime.Now;

                        var kots = await GetOrderKotsAsync(runningOrderId);

                        for (int i = 0; i < kots.Length; i++)
                        {
                            orderModel.TotalPrice += kots[i].TotalPrice;
                            orderModel.TotalItemCount += kots[i].TotalItemCount;
                        }

                        if (db.Update(orderModel) == 0)
                        {
                            errorMesasge = "Error in updating Order";
                            throw new Exception("Error update Order");
                        }
                    }
                    else
                    {
                        errorMesasge = "Error in updating Order";
                    }

                }
            });

            
            return errorMesasge;
        }

        /// <summary>
        /// Get the latest ordernumber from today
        /// </summary>
        /// <returns>Return the last ordernumber</returns>
        public async Task<long> GetLastestOrderNumberForToday()
        {
            var yesterday = DateTime.Today.AddDays(-1);
            var tomorrow = DateTime.Today.AddDays(1);
            var latestOrder = await _connection.Table<Order>().FirstOrDefaultAsync(y => y.OrderDate > yesterday && y.OrderDate < tomorrow);
            if (latestOrder != null) 
                return latestOrder.OrderNumber;
            else
                return 0;
        }

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