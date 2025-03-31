using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Necessary details we need to mark the payment as completed in case of online payment
    /// </summary>
    public class PaymentNecessaryDetail
    {
        /// <summary>
        /// The qr code id to monitor
        /// </summary>
        public string QrCodeId { get; set; }
        /// <summary>
        /// Order id to be setteled
        /// </summary>
        public long Orderid { get; set; }
        /// <summary>
        /// Type of the order
        /// </summary>
        public OrderTypes OrderType { get; set; }
        /// <summary>
        /// Total amount to be paid
        /// </summary>
        public decimal OrderTotal { get; set; }
        /// <summary>
        /// In case of a dine in scenario we need to update the table model through out the application
        /// </summary>
        public TableModel TableModelToUpdate { get; set; } = null;
    }
}
