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
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MenuCategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
