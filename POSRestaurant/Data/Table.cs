using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Table Entity for SQLite
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Table Id
        /// Primary key, autoincrement
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Table number, Id could be different, we can't say
        /// </summary>
        public int TableNo { get; set; }
    }
}
