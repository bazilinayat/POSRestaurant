using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// OrderModel for the UI
    /// </summary>
    public partial class OrderModel : ObservableObject
    {
        /// <summary>
        /// Order Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// OrderNumber each day will start from 1
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
        /// To represent the type of order this is
        /// </summary>
        public string OrderTypeString
        {
            get
            {
                switch(OrderType)
                {
                    case OrderTypes.Online:
                        return "Online";
                    case OrderTypes.DineIn:
                        return "Dine In";
                    case OrderTypes.Pickup:
                        return "Pickup";
                    default:
                        return "Error";
                }
            }
        }
        /// <summary>
        /// Number of people sitting on the table
        /// </summary>
        public int NumberOfPeople { get; set; }
        /// <summary>
        /// Id of the staff waiter who handled the order and table
        /// </summary>
        public int WaiterId { get; set; }
        /// <summary>
        /// Items in the current order
        /// </summary>
        public KOTModel[] KOTs { get; set; }
        /// <summary>
        /// To track the selected order
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
