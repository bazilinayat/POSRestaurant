using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Class to be exclusively used for WeakReferenceMessenger
    /// </summary>
    public class ItemOnMenuChangeModel
    {
        /// <summary>
        /// To know if the attached ItemOnMenu is deleted or not
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// ItemOnMenu changed value
        /// </summary>
        public ItemOnMenuModel ItemModel { get; set; }
    }
}
