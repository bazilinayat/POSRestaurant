using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    public class KOTItemBillModel
    {
        /// <summary>
        /// MenuCategoryItem Id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// Name of the item in the cart
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Icon for the item in the cart
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Price for the item in the cart
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity of the item added to cart
        /// To be changed from UI
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Total amount for the item added to cart
        /// </summary>
        public decimal Amount => Price * Quantity;
    }
}
