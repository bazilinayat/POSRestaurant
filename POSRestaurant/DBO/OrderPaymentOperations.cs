using POSRestaurant.Data;
using SQLite;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle db OrderPayment operations
    /// </summary>
    public class OrderPaymentOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public OrderPaymentOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Method to save the order payment details
        /// </summary>
        /// <param name="orderPayment">OrderPayemnt to save</param>
        /// <returns>Error message string, null on success</returns>
        public async Task<string?> SaveOrderPaymentAsync(OrderPayment orderPayment)
        {
            if (await _connection.InsertAsync(orderPayment) > 0)
                return null;

            return "Error in saving order payment details";
        }

        /// <summary>
        /// To get the order payment details as per the given order id
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <returns>Returns a Order Payment Object</returns>
        public async Task<OrderPayment> GetOrderPaymentById(long orderId) =>
            await _connection.Table<OrderPayment>().FirstOrDefaultAsync(o => o.OrderId == orderId);

        /// <summary>
        /// Get a array of all the order payments, if needed, you can apply paging here
        /// </summary>
        /// <param name="selectedDate">Date filter</param>
        /// <param name="orderType">Order Type filter</param>
        /// <returns>Array of filtered orders</returns>
        public async Task<OrderPayment[]> GetFilteredOrderPaymentsAsync(DateTime selectedDate, int orderType)
        {
            var yesterday = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
            var oneDateMore = selectedDate.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var ordersOnDate = await _connection.Table<OrderPayment>().Where(o => o.SettlementDate > yesterday && o.SettlementDate < tomorrow).ToListAsync();

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
        /// Method to save the order online reference
        /// </summary>
        /// <param name="orderOnlineReference">OrderOnlineReference to save</param>
        /// <returns>False in Error, True on success</returns>
        public async Task<bool> SaveOrderOnlineReferenceAsync(OrderOnlineReference orderOnlineReference)
        {
            if (await _connection.InsertAsync(orderOnlineReference) > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Method to get the order online reference
        /// </summary>
        /// <param name="orderId">OrderOnlineReference OrderId</param>
        /// <returns>OrderOnlineReference object</returns>
        public async Task<OrderOnlineReference> GetOrderOnlineReferenceAsync(long orderId) =>
            await _connection.Table<OrderOnlineReference>().Where(o => o.OrderId == orderId).FirstOrDefaultAsync();

        /// <summary>
        /// Method to delete the order online reference
        /// </summary>
        /// <param name="orderOnlineReference">OrderOnlineReference to delete</param>
        /// <returns>False in Error, True on success</returns>
        public async Task<bool> DeleteOrderOnlineReferenceAsync(OrderOnlineReference orderOnlineReference)
        {
            if (await _connection.DeleteAsync(orderOnlineReference) > 0)
                return true;

            return false;
        }
    }
}
