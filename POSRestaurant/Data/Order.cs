using POSRestaurant.Models;
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
        public decimal TotalAmount { get; set; }
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
        /// <summary>
        /// To know if there was any discount given at the time of this order placement
        /// </summary>
        public bool IsDiscountGiven { get; set; }
        /// <summary>
        /// To know if fixed discount was given on the order
        /// </summary>
        public bool IsFixedBased { get; set; }
        /// <summary>
        /// To know if percentage discount was given on the order
        /// </summary>
        public bool IsPercentageBased { get; set; }
        /// <summary>
        /// To know how much fixed discount was given on the order
        /// </summary>
        public decimal DiscountFixed { get; set; }
        /// <summary>
        /// To know how much percentage discount as given on the order
        /// </summary>
        public decimal DiscountPercentage { get; set; }
        /// <summary>
        /// To know how much customer has to pay after discount
        /// </summary>
        public decimal TotalAmountAfterDiscount { get; set; }
        /// <summary>
        /// To know if the restaurant was using gst at the time of this order placement
        /// </summary>
        public bool UsingGST {  get; set; }
        /// <summary>
        /// CGST percent for order
        /// </summary>
        public decimal CGST {  get; set; }
        /// <summary>
        /// SGST percent for order
        /// </summary>
        public decimal SGST { get; set; }
        /// <summary>
        /// CGST amount for order
        /// </summary>
        public decimal CGSTAmount {  get; set; }
        /// <summary>
        /// SGST amount for order
        /// </summary>
        public decimal SGSTAmount { get; set; }
        /// <summary>
        /// To know how much round off was done to get the grandtotal
        /// </summary>
        public decimal RoundOff { get; set; }
        /// <summary>
        /// Grand total of the order
        /// </summary>
        public decimal GrandTotal { get; set; }

        // For pickup orders only

        /// <summary>
        /// Source of the pickup order, taken from enum
        /// </summary>
        public int Source { get; set; }
        /// <summary>
        /// The reference number for online orders
        /// </summary>
        public string ReferenceNo { get; set; }
        /// <summary>
        /// The id for delivery staff who has delivered the order
        /// </summary>
        public int DeliveryPerson { get; set; }

        /// <summary>
        /// To create Order from order entity
        /// </summary>
        /// <param name="entity">OrderModel entity</param>
        /// <returns>Order object</returns>
        public static Order FromEntity(OrderModel entity) =>
            new()
            {
                Id = entity.Id,
                TableId = entity.TableId,
                OrderDate = entity.OrderDate,
                TotalItemCount = entity.TotalItemCount,
                TotalAmount = entity.TotalAmount,
                PaymentMode = entity.PaymentMode,
                OrderStatus = entity.OrderStatus,
                OrderType = entity.OrderType,
                NumberOfPeople = entity.NumberOfPeople,
                OrderNumber = entity.OrderNumber,
                WaiterId = entity.WaiterId,

                IsDiscountGiven = entity.IsDiscountGiven,
                IsFixedBased = entity.IsFixedBased,
                IsPercentageBased = entity.IsPercentageBased,
                DiscountFixed = entity.DiscountFixed,
                DiscountPercentage = entity.DiscountPercentage,
                TotalAmountAfterDiscount = entity.TotalAmountAfterDiscount,

                UsingGST = entity.UsingGST,
                CGST = entity.CGST,
                SGST = entity.SGST,
                CGSTAmount = entity.CGSTAmount,
                SGSTAmount = entity.SGSTAmount,

                RoundOff = entity.RoundOff,
                GrandTotal = entity.GrandTotal,

                Source = entity.Source,
                ReferenceNo = entity.ReferenceNo,
                DeliveryPerson = entity.DeliveryPerson,
            };
    }
}
