using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// To store the discount details of an order
    /// </summary>
    public class Discount
    {
        /// <summary>
        /// Primary key of the table
        /// </summary>
        [AutoIncrement, PrimaryKey]
        public long Id { get; set; }
        /// <summary>
        /// OrderId to which this discount belongs
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// True, if discount ammount is fixed
        /// </summary>
        public bool IsFixedBased { get; set; }
        /// <summary>
        /// True, if discount is given in percentage
        /// </summary>
        public bool IsPercentageBased { get; set; }
        /// <summary>
        /// Only applicable if IsFixedBased is true
        /// The fixed discount given on order
        /// </summary>
        public decimal DiscountFixed { get; set; }
        /// <summary>
        /// Only applicable if IsPercentageBased is true
        /// The percentage discount given on order
        /// </summary>
        public decimal DiscountPercentage { get; set; }
    }
}
