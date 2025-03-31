using POSRestaurant.Data;
using POSRestaurant.Models;
using POSRestaurant.Service;
using POSRestaurant.Service.SettingService;
using SQLite;

namespace POSRestaurant.DBO
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
        private readonly Setting _settingService;

        /// <summary>
        /// Variable for getting initial data
        /// </summary>
        private readonly SeedData _seedData;

        /// <summary>
        /// To handle calls to staff operations in the db
        /// </summary>
        public StaffOperations StaffOperaiotns;

        /// <summary>
        /// To handle calls to staff operations in the db
        /// </summary>
        public OrderPaymentOperations OrderPaymentOperations;

        /// <summary>
        /// To handle calls to menu operations in the db
        /// </summary>
        public MenuOperations MenuOperations;

        /// <summary>
        /// To handle calls to menu operations in the db
        /// </summary>
        public TableOperations TableOperations;

        /// <summary>
        /// To handle calls to inventory related operations in the db
        /// </summary>
        public InventoryOperations InventoryOperations;

        /// <summary>
        /// To handle calls to discount related operations in the db
        /// </summary>
        public DiscountOperations DiscountOperations;
        
        /// <summary>
        /// To handle calls to settings related operations in the db
        /// </summary>
        public SettingsOperation SettingsOperation;

        /// <summary>
        /// To handle calls to users related operations in the db
        /// </summary>
        public UserOperations UserOperation;

        /// <summary>
        /// A list of different entity types we have
        /// </summary>
        private readonly List<Type> _entityTypes;

        /// <summary>
        /// The migration manager used for altering or creating new tables.
        /// </summary>
        private MigrationManager _migrationManager;

        /// <summary>
        /// Class constructor, to generate database and connection
        /// </summary>
        /// <param name="settingService">DI for SettingService</param>
        public DatabaseService(Setting settingService)
        {
            _settingService = settingService;
            _seedData = new SeedData(_settingService);

            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"))) 
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
            }

            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data",  "RestPOS.db3");
            _connection = new SQLiteAsyncConnection(dbPath, 
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

            StaffOperaiotns = new StaffOperations(_connection);
            OrderPaymentOperations = new OrderPaymentOperations(_connection);
            MenuOperations = new MenuOperations(_connection);
            TableOperations = new TableOperations(_connection);
            InventoryOperations = new InventoryOperations(_connection);
            DiscountOperations = new DiscountOperations(_connection);
            SettingsOperation = new SettingsOperation(_connection);
            UserOperation = new UserOperations(_connection);

            // Register all your entity types here
            _entityTypes = new List<Type>
            {
                typeof(MenuCategory),
                typeof(ItemOnMenu),
                typeof(Order),
                typeof(OrderItem),
                typeof(Discount),
                typeof(Table),
                typeof(KOT),
                typeof(KOTItem),
                typeof(Staff),
                typeof(ExpenseItem),
                typeof(Inventory),
                typeof(OrderPayment),
                typeof(RestaurantInfo),
                typeof(ExpenseTypes),
                typeof(TableState),
                typeof(Permission),
                typeof(UserRole),
                typeof(RolePermission),
                typeof(User),
                typeof(AssignedUserRole),
                typeof(OrderOnlineReference)
            };

            // Create the migration manager
            _migrationManager = new MigrationManager(dbPath, _entityTypes);
        }

        /// <summary>
        /// Method to initialize database - create necessary tables.
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async Task InitializeDatabaseAsync()
        {
            // Create tables for your entities if they don't exist
            foreach (var entityType in _entityTypes)
            {
                await _connection.CreateTableAsync(entityType, CreateFlags.None);
            }

            // Run automated migrations
            await _migrationManager.MigrateAsync();

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
            if (firstCategory == null)
            {
                var categories = _seedData.GetMenuCategories();
                var menuItems = _seedData.GetMenuItems();
                var tables = _seedData.GetTables();

                await _connection.InsertAllAsync(categories);
                await _connection.InsertAllAsync(menuItems);
                await _connection.InsertAllAsync(tables);
            }

            var firstDelivery = await _connection.Table<Staff>().FirstOrDefaultAsync();
            if (firstDelivery == null)
            {
                await StaffOperaiotns.SaveStaffAsync(new StaffModel { Id = 0, Name = "Swiggy", PhoneNumber = "NA", Role = StaffRole.Delivery });
                await StaffOperaiotns.SaveStaffAsync(new StaffModel { Id = 0, Name = "Zomato", PhoneNumber = "NA", Role = StaffRole.Delivery });
                await StaffOperaiotns.SaveStaffAsync(new StaffModel { Id = 0, Name = "Self", PhoneNumber = "NA", Role = StaffRole.Delivery });
            }

            var firstPermission = await _connection.Table<Permission>().FirstOrDefaultAsync();
            if (firstPermission == null)
            {
                // Permissions, Role and Admin 
                await UserOperation.SavePermissionAsync("MakeOrders");
                await UserOperation.SavePermissionAsync("ViewOrders");
                await UserOperation.SavePermissionAsync("EditOrders");
                await UserOperation.SavePermissionAsync("EditMenu");
                await UserOperation.SavePermissionAsync("EditStaff");
                await UserOperation.SavePermissionAsync("ViewReport");
                await UserOperation.SavePermissionAsync("EditInventory");
                await UserOperation.SavePermissionAsync("EditRoles");
                await UserOperation.SavePermissionAsync("EditUser");
            }

            if ((await _connection.Table<UserRole>().Where(o => o.Name == "Admin").FirstOrDefaultAsync() == null))
            {
                var adminRole = new UserRoleEditModel
                {
                    Name = "Admin",
                    Permissions = new List<PermissionModel>()
                };
                var permissions = await UserOperation.GetAllPermissionsAsync();
                foreach (var permission in permissions)
                {
                    adminRole.Permissions.Add(new PermissionModel
                    {
                        Id = permission.Id,
                        Name = permission.Name,
                        IsSelected = true
                    });
                }
                await UserOperation.SaveRoleAsync(adminRole);
            }

            if ((await _connection.Table<User>().Where(o => o.Username == "Admin").FirstOrDefaultAsync() == null))
            {
                await UserOperation.SaveUserAsync(new UserEditModel
                {
                    Username = "Admin",
                    Password = "Admin",
                    AssignedRoleId = 1
                });
            }
        }

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
                TotalAmount = orderModel.TotalAmount,
                OrderStatus = orderModel.OrderStatus,
                OrderNumber = orderModel.OrderNumber,
                OrderType = orderModel.OrderType,
                NumberOfPeople = orderModel.NumberOfPeople,
                WaiterId = orderModel.WaiterId,
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
                    orderModel.TotalAmount += kotItem.TotalPrice;
                    orderModel.TotalItemCount += kotItem.TotalItemCount;
                    orderModel.OrderDate = DateTime.Now;
                }
            }

            // Update the order object with latest details
            await _connection.UpdateAsync(orderModel);

            return null;
        }

        /// <summary>
        /// Method to place pickup orders
        /// </summary>
        /// <param name="orderModel">Order to be placed, with items</param>
        /// <returns>Returns Error Message or null (in case of success)</returns>
        public async Task<string?> PlacePickupOrderAsync(OrderModel orderModel)
        {
            var order = new Order()
            {
                OrderNumber = orderModel.OrderNumber,
                TableId = orderModel.TableId,
                OrderDate = orderModel.OrderDate,
                TotalItemCount = orderModel.TotalItemCount,
                TotalAmount = orderModel.TotalAmount,
                PaymentMode = orderModel.PaymentMode,
                OrderStatus = orderModel.OrderStatus,
                OrderType = orderModel.OrderType,
                
                NumberOfPeople = orderModel.NumberOfPeople,
                WaiterId = orderModel.WaiterId,
                
                IsDiscountGiven = orderModel.IsDiscountGiven,
                IsFixedBased = orderModel.IsFixedBased,
                IsPercentageBased = orderModel.IsPercentageBased,
                DiscountFixed = orderModel.DiscountFixed,
                DiscountPercentage = orderModel.DiscountPercentage,
                TotalAmountAfterDiscount = orderModel.TotalAmountAfterDiscount,
                
                UsingGST = orderModel.UsingGST,
                CGST = orderModel.CGST,
                SGST = orderModel.SGST,
                CGSTAmount = orderModel.CGSTAmount,
                SGSTAmount = orderModel.SGSTAmount,

                RoundOff = orderModel.RoundOff,
                GrandTotal = orderModel.GrandTotal,

                Source = orderModel.Source,
                ReferenceNo = orderModel.ReferenceNo,
                DeliveryPerson = orderModel.DeliveryPerson,
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
        /// Delete order based on given Order
        /// </summary>
        /// <param name="order">Order to delete</param>
        /// <returns>Returns a Task object</returns>
        public async Task DeleteOrderAsync(Order order) =>
            await _connection.DeleteAsync(order);


        /// <summary>
        /// Delete order based on given OrderId
        /// </summary>
        /// <param name="orderId">Id for Order to delete</param>
        /// <returns>Returns a Task object</returns>
        public async Task<bool> DeleteCompleteOrderAsync(long orderId)
        {
            try
            {
                var kots = await GetOrderKotsAsync(orderId);

                await _connection.RunInTransactionAsync(async (transaction) =>
                {
                    foreach(var kot in kots)
                    {
                        // Delete the KOTItems for our specific KOTs
                        await _connection.ExecuteAsync("DELETE FROM KOTItem WHERE KOTId = ?", kot.Id);

                        // Delete the KOT from Ids
                        await _connection.ExecuteAsync("DELETE FROM KOT WHERE Id = ?", kot.Id);
                    }

                    // Delete the role
                    await _connection.ExecuteAsync("DELETE FROM [Order] WHERE Id = ?", orderId);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

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
        /// To update the given order
        /// </summary>
        /// <param name="order">order to update</param>
        /// <returns>Returns the number of rows affected</returns>
        public async Task<int> UpdateOrder(Order order) =>
            await _connection.UpdateAsync(order);

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
        /// Get a array of all the orders, if needed, you can apply paging here
        /// </summary>
        /// <param name="selectedDate">Date filter</param>
        /// <param name="orderType">Order Type filter</param>
        /// <returns>Array of filtered orders</returns>
        public async Task<Order[]> GetFilteredOrderssAsync(DateTime selectedDate, int orderType)
        {
            var yesterday = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
            var oneDateMore = selectedDate.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var ordersOnDate = await _connection.Table<Order>().Where(o => o.OrderDate > yesterday && o.OrderDate < tomorrow && o.OrderStatus == TableOrderStatus.Paid).ToListAsync();
            
            if (orderType == 0)
            {
                return ordersOnDate.ToArray();
            } 
            else
            {
                return ordersOnDate.Where(o => o.OrderType == (OrderTypes)orderType).ToArray();
            }
        }
            

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
        /// Method to get all the items in Kot
        /// </summary>
        /// <param name="selectedDate">Selected date for report/param>
        /// <returns>Array of KotItem</returns>
        public async Task<KOTItem[]> GetAllKotItemsAsync(DateTime selectedDate)
        {
            var yesterday = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
            var oneDateMore = selectedDate.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var ordersOnDate = await _connection.Table<Order>().Where(o => o.OrderDate > yesterday && o.OrderDate < tomorrow).ToListAsync();

            var orderIds = ordersOnDate.Select(o => o.Id).ToArray();

            var kotsOnDate = await _connection.Table<KOT>().Where(o => orderIds.Contains(o.OrderId)).ToArrayAsync();

            var kotIds = kotsOnDate.Select(o => o.Id).ToArray();

            var kotItems = await _connection.Table<KOTItem>().Where(o => kotIds.Contains(o.KOTId)).ToArrayAsync();
            
            return kotItems;
        }

        /// <summary>
        /// To Get all the menu items in database
        /// </summary>
        /// <returns>Returns a array of ItemOnMenu</returns>
        public async Task<ItemOnMenu[]> GetMenuItemBySearch() =>
            await _connection.Table<ItemOnMenu>().ToArrayAsync();

        /// <summary>
        /// To Get menu item by name thorugh searching in database
        /// </summary>
        /// <param name="searchText">The item name to search</param>
        /// <returns>Returns a array of ItemOnMenu</returns>
        public async Task<ItemOnMenu[]> GetMenuItemBySearch(string searchText) =>
            await _connection.Table<ItemOnMenu>().Where(o => o.ShortCode.Contains(searchText) || o.Name.ToLower().Contains(searchText.ToLower())).ToArrayAsync();

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
                        orderModel.TotalAmount = 0;
                        orderModel.TotalItemCount = 0;
                        orderModel.OrderDate = DateTime.Now;

                        var kots = await GetOrderKotsAsync(runningOrderId);

                        for (int i = 0; i < kots.Length; i++)
                        {
                            orderModel.TotalAmount += kots[i].TotalPrice;
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
                        orderModel.TotalAmount = 0;
                        orderModel.TotalItemCount = 0;
                        orderModel.OrderDate = DateTime.Now;

                        var kots = await GetOrderKotsAsync(runningOrderId);

                        for (int i = 0; i < kots.Length; i++)
                        {
                            orderModel.TotalAmount += kots[i].TotalPrice;
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
            var yesterday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            var oneDateMore = DateTime.Today.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var latestOrder = await _connection.Table<Order>().Where(y => y.OrderDate > yesterday && y.OrderDate < tomorrow && y.OrderType != OrderTypes.Pickup).OrderByDescending(o => o.OrderDate).FirstOrDefaultAsync();
            if (latestOrder != null) 
                return latestOrder.OrderNumber;
            else
                return 0;
        }

        /// <summary>
        /// Get the latest pickup ordernumber from today
        /// </summary>
        /// <returns>Return the last pickup ordernumber</returns>
        public async Task<long> GetLastestPickupOrderNumberForToday()
        {
            var yesterday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            var oneDateMore = DateTime.Today.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var latestOrder = await _connection.Table<Order>().Where(y => y.OrderDate > yesterday && y.OrderDate < tomorrow && y.OrderType == OrderTypes.Pickup).OrderByDescending(o => o.OrderDate).FirstOrDefaultAsync();
            if (latestOrder != null)
                return latestOrder.OrderNumber;
            else
                return 0;
        }

        /// <summary>
        /// Get the list of all the running pickup orders
        /// </summary>
        /// <returns>Returns a list of Order object</returns>
        public async Task<Order[]> GetRunningPickupOrders() =>
            await _connection.Table<Order>().Where(o => o.OrderType == OrderTypes.Pickup && o.OrderStatus == TableOrderStatus.Running).ToArrayAsync();

        /// <summary>
        /// Get the list of all the running orders
        /// </summary>
        /// <returns>Returns a list of Order object</returns>
        public async Task<Order[]> GetRunningOrdersAsync() =>
            await _connection.Table<Order>().Where(o => o.OrderStatus == TableOrderStatus.Running).ToArrayAsync();

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
