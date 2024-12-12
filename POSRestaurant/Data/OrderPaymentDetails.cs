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
        /// Total amount of the order before tax
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Total amout of the order after tax
        /// To be paid by the customer
        /// </summary>
        public decimal Total {  get; set; }
        /// <summary>
        /// PaymentMode, which was used to pay
        /// </summary>
        public PaymentModes PaymentMode { get; set; }
        /// <summary>
        /// PaymentMode, which was used to pay
        /// </summary>
        public decimal AmountPaid { get; set; }
    }
}
