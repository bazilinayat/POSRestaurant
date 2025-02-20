using POSRestaurant.Data;
using SQLite;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle all the table related operations
    /// </summary>
    public class TableOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public TableOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Method to get all the tables from database
        /// </summary>
        /// <returns>Array of Table</returns>
        public async Task<Table[]> GetTablesAsync() =>
            await _connection.Table<Table>().ToArrayAsync();

        /// <summary>
        /// Method to get table number from database
        /// </summary>
        /// <param name="tableId">Table Id for which we need table number</param>
        /// <returns>Int, table number</returns>
        public async Task<int> GetTableNoAsync(int tableId) =>
            (await _connection.Table<Table>().FirstOrDefaultAsync(o => o.Id == tableId)).TableNo;

        /// <summary>
        /// Method to get all the tables from database
        /// </summary>
        /// <returns>Array of Table</returns>
        public async Task<string?> AddNewTableAsync()
        {
            string? errorMessage = null;
            try
            {
                var tables = await GetTablesAsync();

                Table table = new Table
                {
                    TableNo = tables.Count() + 1
                };

                await _connection.InsertAsync(table);

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
