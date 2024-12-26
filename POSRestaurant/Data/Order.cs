using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Order Entity for SQLite
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Order Id
        /// Primary key, auto incremented
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// Order numbers each day start from 1
        /// </summary>
        public long OrderNumber { get; set; }
        /// <summary>
        /// Table Id, Order belongs to
        /// </summary>
        public int TableId { get; set; }
        /// <summary>
        /// DateTime of the Order
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Total number of items of the order
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// PaymentMode, which was used to pay
        /// </summary>
        public PaymentModes PaymentMode { get; set; }
        /// <summary>
        /// To represent current order status
        /// </summary>
        public TableOrderStatus OrderStatus { get; set; }
        /// <summary>
        /// To represent the type of order this is
        /// </summary>
        public OrderTypes OrderType { get; set; }
        /// <summary>
        /// Number of people sitting on the table
        /// </summary>
        public int NumberOfPeople { get; set; }
        /// <summary>
        /// Id of the staff waiter who handled the order and table
        /// </summary>
        public int WaiterId { get; set; }
    }
}
