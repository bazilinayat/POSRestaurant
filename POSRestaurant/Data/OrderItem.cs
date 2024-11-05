using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// OrderItem Entity for SQLite
    /// </summary>
    public class OrderItem
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Icom { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Ignore]
        public decimal Amount => Quantity * Price;
    }
}
