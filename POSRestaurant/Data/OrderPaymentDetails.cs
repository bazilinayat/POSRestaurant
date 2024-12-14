using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Class to store the order payment details
    /// </summary>
    public class OrderPayment
    {
        /// <summary>
        /// OrderPayment Id
        /// Primary key, auto incremented
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// Associated Order Id
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// Total amout of the order after tax
        /// To be paid by the customer
        /// </summary>
        public decimal Total {  get; set; }
        /// <summary>
        /// Total amout of the order after tax
        /// To be paid by the customer
        /// </summary>
        public decimal PaidByCustomer { get; set; }
        /// <summary>
        /// Total amout of the order after tax
        /// To be paid by the customer
        /// </summary>
        [Ignore]
        public decimal Return => PaidByCustomer - Total;
        /// <summary>
        /// Total amout of the order after tax
        /// To be paid by the customer
        /// </summary>
        public decimal Tip { get; set; }
        /// <summary>
        /// PaymentMode, which was used to pay
        /// </summary>
        public PaymentModes PaymentMode { get; set; }
    }
}
