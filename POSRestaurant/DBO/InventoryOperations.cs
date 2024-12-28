using POSRestaurant.Data;
using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle all the database inventory operations
    /// </summary>
    public class InventoryOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public InventoryOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Get all the expense items as per the expense type given
        /// </summary>
        /// <param name="expenseType">Type to be searched</param>
        /// <returns>A array of Expense items</returns>
        public async Task<ExpenseItem[]> GetExpenseItemBasedOnTypeAsync(ExpenseItemTypes expenseType) =>
            await _connection.Table<ExpenseItem>().Where(o => o.ItemType == expenseType).ToArrayAsync();

        /// <summary>
        /// Get the expense item as per the id given
        /// </summary>
        /// <param name="Id">Id to be searched</param>
        /// <returns>A Expense Item</returns>
        public async Task<ExpenseItem> GetExpenseItemBasedOnIdAsync(int Id) =>
            await _connection.Table<ExpenseItem>().FirstOrDefaultAsync(o => o.Id == Id);

        /// <summary>
        /// Get all the expense items
        /// </summary>
        /// <returns>A array of Expense items</returns>
        public async Task<ExpenseItem[]> GetAllExpenseItemsAsync() =>
            await _connection.Table<ExpenseItem>().ToArrayAsync();

        /// <summary>
        /// Method to save or update the expense item record
        /// </summary>
        /// <param name="expenseItemToSave">ExpenseItemModel object to be saved</param>
        /// <returns>Error message string, null on success</returns>
        public async Task<string?> SaveStaffAsync(ExpenseItemModel expenseItemToSave)
        {
            ExpenseItem expenseItem = new ExpenseItem
            {
                Id = expenseItemToSave.Id,
                Name = expenseItemToSave.Name,
                ItemType = expenseItemToSave.ItemType,
                IsWeighted = expenseItemToSave.IsWeighted,
            };

            if (expenseItem.Id == 0)
            {
                if (await _connection.InsertAsync(expenseItem) > 0)
                    return null;

                return "Error in saving Expense Item";
            }
            else
            {
                if (await _connection.UpdateAsync(expenseItem) > 0)
                    return null;

                return "Error in updating Expense Item";
            }
        }

        /// <summary>
        /// Delete Expense Item from the database
        /// </summary>
        /// <param name="expenseItemModel">Expense Item to delete</param>
        /// <returns>Returns the number of rows deleted</returns>
        public async Task<int> DeleteStaffAsync(ExpenseItemModel expenseItemModel)
        {
            ExpenseItem expenseItem = new ExpenseItem
            {
                Id = expenseItemModel.Id,
                Name = expenseItemModel.Name,
                ItemType = expenseItemModel.ItemType,
                IsWeighted = expenseItemModel.IsWeighted,
            };

            return await _connection.DeleteAsync(expenseItem);
        }
    }
}
