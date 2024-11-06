using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// OrderItem Entity for SQLite
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// OrderItem Id
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// OrderId to which this item belongs to
        /// </summary>
        public long OrderId { get; set; }
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
        public string Icom { get; set; }
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
    }
}
