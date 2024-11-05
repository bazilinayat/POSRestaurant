using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// MenuCategory Entity for SQLite
    /// </summary>
    public class MenuCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
