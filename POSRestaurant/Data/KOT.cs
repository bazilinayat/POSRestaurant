using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Order Entity for SQLite
    /// </summary>
    public class KOT
    {
        /// <summary>
        /// Order Id
        /// Primary key, auto incremented
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// To keep track of the number of KOTs in a order
        /// </summary>
        public int KOTNumber { get; set; }
        /// <summary>
        /// Order Id, KOT belongs to
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// DateTime of the Order
        /// </summary>
        public DateTime KOTDateTime { get; set; }
        /// <summary>
        /// Total number of items of the order
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// To manage getting the kot items quickly
        /// </summary>
        [Ignore]
        public KOTItem[] Items { get; set; }
    }
}
