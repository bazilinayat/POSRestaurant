using POSRestaurant.Data;
using POSRestaurant.Models;
using SQLite;
using Windows.System.Profile;

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

        /// <summary>
        /// To save the inventory entry in the DB
        /// </summary>
        /// <param name="inventoryToSave">Inventory entry to save</param>
        /// <returns>Return error message in failure, else null</returns>
        public async Task<string?> SaveInventoryEntryAsync(Inventory inventoryToSave)
        {
            if (inventoryToSave.Id == 0)
            {
                if (await _connection.InsertAsync(inventoryToSave) > 0)
                    return null;

                return "Error in saving Inventory Entry";
            }
            else
            {
                if (await _connection.UpdateAsync(inventoryToSave) > 0)
                    return null;

                return "Error in updating Inventory Entry";
            }
        }

        /// <summary>
        /// To get all the inventory items from the db
        /// </summary>
        /// <returns>Array of Inventories</returns>
        public async Task<Inventory[]> GetAllInventoryItemAsync() =>
            await _connection.Table<Inventory>().ToArrayAsync();

        /// <summary>
        /// To filter the data for reports as per the conditions given
        /// </summary>
        /// <param name="selectedDate">Selected date of the data entry</param>
        /// <param name="expenseType">Int, Expense type of the entry</param>
        /// <param name="paidByWho">Paid by who, staffId of person</param>
        /// <returns>Array of Inventory</returns>
        public async Task<Inventory[]> GetInventoryItemsAsync(DateTime selectedDate, long expenseType, int paidByWho)
        {
            var yesterday = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
            var oneDateMore = selectedDate.AddDays(1);
            var tomorrow = new DateTime(oneDateMore.Year, oneDateMore.Month, oneDateMore.Day, 0, 0, 0);

            var inventoryOnDate = await _connection.Table<Inventory>().Where(o => o.EntryDate > yesterday && o.EntryDate < tomorrow).ToArrayAsync();

            var filteredData = inventoryOnDate;

            if (expenseType != 0)
            {
                filteredData = filteredData.Where(o => o.ExpenseTypeId == expenseType).ToArray();
            }

            if (paidByWho != 0)
            {
                filteredData = filteredData.Where(o => o.StaffId == paidByWho).ToArray();
            }

            return filteredData;
        }

    }
}
