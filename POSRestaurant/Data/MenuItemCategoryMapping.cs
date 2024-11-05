using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// MenuItemCategoryMapping - Third Table for SQLite
    /// </summary>
    public class MenuItemCategoryMapping
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MenuCategoryId { get; set; }
        public int MenuItemId { get; set; }
    }
}
