using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// class to store details of expense item
    /// </summary>
    public class ExpenseItem
    {
        /// <summary>
        /// Primary id of the items
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item type
        /// </summary>
        public ExpenseItemTypes ItemType { get; set; }

        /// <summary>
        /// True if item is to be weighted
        /// False if quntity
        /// </summary>
        public bool IsWeighted { get; set; }
    }
}
