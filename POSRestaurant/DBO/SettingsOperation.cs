using POSRestaurant.Data;
using SQLite;
using Windows.UI.Core.AnimationMetrics;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle db staff operations
    /// </summary>
    public class SettingsOperation
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public SettingsOperation(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// There is only oen record in the database in this table
        /// This will get that one database entry
        /// </summary>
        /// <returns></returns>
        public async Task<RestaurantInfo> GetRestaurantInfo() =>
            await _connection.Table<RestaurantInfo>().FirstOrDefaultAsync();

        /// <summary>
        /// To save the restaurant info
        /// </summary>
        /// <param name="info">Restaurant Info to be saved in the database</param>
        /// <returns>Returns error message in failure, else null</returns>
        public async Task<string?> SaveRestaurantInfo(RestaurantInfo info)
        {
            int count = await _connection.Table<RestaurantInfo>().CountAsync();
            if (count > 0)
            {
                info.Id = 1;
                if (await _connection.UpdateAsync(info) > 0)
                    return null;

                return "Error in updating restaurant info";
            }
            else
            {
                if (await _connection.InsertAsync(info) > 0)
                    return null;

                return "Error in saving restaurant info";
            }
        }

        /// <summary>
        /// To get all the expense types 
        /// </summary>
        /// <returns></returns>
        public async Task<ExpenseTypes[]> GetExpenseTypes() =>
            await _connection.Table<ExpenseTypes>().ToArrayAsync();

        /// <summary>
        /// To save the expense type
        /// </summary>
        /// <param name="info">Restaurant Info to be saved in the database</param>
        /// <returns>Returns error message in failure, else null</returns>
        public async Task<string?> SaveExpenseType(string name)
        {
            var expenseType = await _connection.Table<ExpenseTypes>().Where(o => o.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
            if (expenseType != null)
            {
                return "Expense Type Already Exists!";
            }
            else
            {
                if (await _connection.InsertAsync(new ExpenseTypes { Name = name }) > 0)
                    return null;

                return "Error in Saving Expense Type";
            }
        }

        /// <summary>
        /// To delete the exisitng expense type
        /// </summary>
        /// <param name="expenseType">Expense type to delete</param>
        /// <returns>Returns error message in failure, else null</returns>
        public async Task<string?> DeleteExpenseType(ExpenseTypes expenseType)
        {
            if (await _connection.DeleteAsync(expenseType) > 0)
                return null;

            return "Error in Deleting Expense Type";
        }
    }
}
