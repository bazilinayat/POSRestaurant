using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Order Entity for SQLite
    /// </summary>
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalItemCount { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentModes PaymentMode { get; set; }
    }
}
