using POSRestaurant.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle all the database discount operations
    /// </summary>
    public class DiscountOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public DiscountOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// To save the discount details for the order
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public async Task<int> SaveDiscountAsync(Discount orderDiscount) =>
            await _connection.InsertAsync(orderDiscount);

        /// <summary>
        /// Get the discount object as per the orderId given
        /// </summary>
        /// <param name="orderId">OrderId for discount</param>
        /// <returns>A object of Discount</returns>
        public async Task<Discount> GetDiscountDetailsForOrderAsync(long orderId) =>
            await _connection.Table<Discount>().Where(o => o.OrderId == orderId).FirstOrDefaultAsync();
    }
}
