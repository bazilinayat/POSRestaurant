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
    }
}
