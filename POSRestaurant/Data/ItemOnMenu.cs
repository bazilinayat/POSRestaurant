using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// MenuItem Entity for SQLite
    /// </summary>
    /// 
    [Table("MenuItem")]
    public class ItemOnMenu
    {
        /// <summary>
        /// MenuCategoryItem Id
        /// PrimaryKey and auto incremented
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// MenuCategory Id to which this item belongs to
        /// </summary>
        public int MenuCategoryId { get; set; }
        /// <summary>
        /// Menu Category Item Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Menu Category Item Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Menu Cateogry Item Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Menu Category Item Price
        /// </summary>
        public decimal Price { get; set; }
    }
}
