using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Class to keep track of changes made to kot items
    /// </summary>
    public class KOTItemChangeTrackingModel
    {
        /// <summary>
        /// KotId for tracking changes
        /// </summary>
        public long KOTId { get; set; }
        /// <summary>
        /// KOTItemModel for tracking changes
        /// </summary>
        public KOTItemModel KotItem { get; set; }
        /// <summary>
        /// To know if quantity is updated
        /// </summary>
        public bool IsQuantityUpdated { get; set; }
        /// <summary>
        /// New updated quantity
        /// </summary>
        public int UpdatedQuantity { get; set; }
        /// <summary>
        /// To know if this item is removed from order
        /// </summary>
        public bool IsItemRemoved { get; set; }
    }
}
