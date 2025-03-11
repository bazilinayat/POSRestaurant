using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// OrderItem Entity for SQLite
    /// </summary>
    public class KOTItem
    {
        /// <summary>
        /// OrderItem Id
        /// </summary>
        [PrimaryKey, AutoIncrement]
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
        /// MenuCategoryItem Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// MenuCategoryItem Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// MenuCategoryItem Price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity of MenuCategoryItem in the Order
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Item amount calculated - Q * P
        /// Ignored, not to be put in DB
        /// </summary>
        [Ignore]
        public decimal Amount => Quantity * Price;
        /// <summary>
        /// To make object of KOTItem
        /// </summary>
        /// <param name="entity">KOTItemModel Object</param>
        /// <returns>Returns a KOTItem object</returns>
        public static KOTItem FromEntity(KOTItemModel entity) =>
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
