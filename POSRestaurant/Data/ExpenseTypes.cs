using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// table to keep the expense types to expese report
    /// </summary>
    public class ExpenseTypes
    {
        /// <summary>
        /// the primary of the table
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// the name of the expense type
        /// </summary>
        public string Name { get; set; }
    }
}
