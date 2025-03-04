using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// MenuCategory Entity for SQLite
    /// </summary>
    public class MenuCategory
    {
        /// <summary>
        /// Menu Category Id
        /// Primary Key, autoincremented
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Menu Category Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Menu Category Icon Name
        /// </summary>
        public string Icon { get; set; }
    }
}
