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
        /// To know the date of the settlement
        /// </summary>
        public DateTime SettlementDate { get; set; }
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
        /// OrderType, which was used to pay
        /// </summary>
        public OrderTypes OrderType { get; set; }
        /// <summary>
        /// PaymentMode, which was used to pay
        /// </summary>
        public PaymentModes PaymentMode { get; set; }
        /// <summary>
        /// If part payment is done, is it done in cash
        /// </summary>
        public bool IsCashForPart { get; set; }
        /// <summary>
        /// If part payment is done, how much is done in cash
        /// </summary>
        public decimal PartPaidInCash { get; set; }
        /// <summary>
        /// If part payment is done, is it done in card
        /// </summary>
        public bool IsCardForPart { get; set; }
        /// <summary>
        /// If part payment is done, how much is done in card
        /// </summary>
        public decimal PartPaidInCard { get; set; }
        /// <summary>
        /// If part payment is done, is it done online
        /// </summary>
        public bool IsOnlineForPart { get; set; }
        /// <summary>
        /// If part payment id done, how much is done online
        /// </summary>
        public decimal PartPaidInOnline { get; set; }
    }
}
