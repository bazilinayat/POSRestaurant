using POSRestaurant.Data;
using SQLite;

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
    }
}
