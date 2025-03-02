using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// To represent database table keeping track of all the waiters and their information
    /// </summary>
    public class Staff
    {
        /// <summary>
        /// primary key of the waiter, to identify the records
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Name of the waiter
        /// </summary>
        [Unique]
        public string Name { get; set; }
        /// <summary>
        /// Mobile number of the waiter
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Role of the staff member
        /// </summary>
        public StaffRole Role { get; set; }
    }
}
