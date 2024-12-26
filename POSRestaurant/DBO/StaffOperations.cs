using POSRestaurant.Data;
using POSRestaurant.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle db staff operations
    /// </summary>
    public class StaffOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public StaffOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Get all the staff members as per the role given
        /// </summary>
        /// <param name="role">Role to be searched</param>
        /// <returns>A array of Staff members</returns>
        public async Task<Staff[]> GetStaffBasedOnRole(StaffRole role) =>
            await _connection.Table<Staff>().Where(o => o.Role == role).ToArrayAsync();

        /// <summary>
        /// Get the staff member as per the id given
        /// </summary>
        /// <param name="Id">Id to be searched</param>
        /// <returns>A Staff member</returns>
        public async Task<Staff> GetStaffBasedOnId(int Id) =>
            await _connection.Table<Staff>().FirstOrDefaultAsync(o => o.Id == Id);

        /// <summary>
        /// Get the staff member name as per the id given
        /// </summary>
        /// <param name="Id">Id to be searched</param>
        /// <returns>Staff member name</returns>
        public async Task<string> GetStaffNameBasedOnId(int Id) =>
            (await _connection.Table<Staff>().FirstOrDefaultAsync(o => o.Id == Id)).Name;

        /// <summary>
        /// Get all the staff members
        /// </summary>
        /// <returns>A array of Staff members</returns>
        public async Task<Staff[]> GetAllStaff() =>
            await _connection.Table<Staff>().ToArrayAsync();

        /// <summary>
        /// Method to save or update the staff member record
        /// </summary>
        /// <param name="staffToSave">StaffModel object to be saved</param>
        /// <returns>Error message string, null on success</returns>
        public async Task<string?> SaveStaffAsync(StaffModel staffToSave)
        {
            Staff staff = new Staff
            {
                Id = staffToSave.Id,
                Name = staffToSave.Name,
                PhoneNumber = staffToSave.PhoneNumber,
                Role = staffToSave.Role
            };

            if (staff.Id == 0)
            {
                if (await _connection.InsertAsync(staff) > 0)
                    return null;

                return "Error in saving staff member";
            }
            else
            {
                if (await _connection.UpdateAsync(staff) > 0)
                    return null;

                return "Error in updating staff member";
            }
        }

        /// <summary>
        /// Delete Staff from the database
        /// </summary>
        /// <param name="staffToDelete">Staff to delete</param>
        /// <returns>Returns the number of rows deleted</returns>
        public async Task<int> DeleteStaffAsync(StaffModel staffToDelete)
        {
            Staff staff = new Staff
            {
                Id = staffToDelete.Id,
                Name = staffToDelete.Name,
                PhoneNumber = staffToDelete.PhoneNumber,
                Role = staffToDelete.Role
            };

            return await _connection.DeleteAsync(staff);
        }
    }
}
