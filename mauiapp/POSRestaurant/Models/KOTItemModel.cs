using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Class to keep track of the records in order
    /// </summary>
    public partial class KOTItemModel : ObservableObject
    {
        /// <summary>
        /// Item Id in the cart
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// OrderId to which this item belongs to
        /// </summary>
        public long KOTId { get; set; }
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
        [ObservableProperty, NotifyPropertyChangedFor(nameof(Amount))]
        public int _quantity;
        /// <summary>
        /// Total amount for the item added to cart
        /// </summary>
        public decimal Amount => Price * Quantity;

        /// <summary>
        /// To make object of MenuCategoryModel
        /// </summary>
        /// <param name="entity">MenuCategory Object</param>
        /// <returns>Returns a MenuCategoryModel object</returns>
        public static KOTItemModel FromEntity(KOTItem entity) =>
            new()
            {
                Id = entity.Id,
                KOTId = entity.KOTId,
                ItemId = entity.ItemId,
                Name = entity.Name,
                Price = entity.Price,
                Icon = entity.Icon,
                Quantity = entity.Quantity,
            };
    }
}
