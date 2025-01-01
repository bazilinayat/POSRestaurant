namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// To store the discount details of an order
    /// </summary>
    public class DiscountModel
    {
        /// <summary>
        /// True, if discount is given 
        /// </summary>
        public bool IsDiscountGiven { get; set; }
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
        public decimal DiscountDixed { get; set; }
        /// <summary>
        /// Only applicable if IsPercentageBased is true
        /// The percentage discount given on order
        /// </summary>
        public decimal DiscountPercentage { get; set; }
    }
}
