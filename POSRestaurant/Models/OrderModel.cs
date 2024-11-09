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
    public partial class OrderModel : ObservableObject
    {
        /// <summary>
        /// Order Id
        /// </summary>
        public long Id { get; set; }
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
        /// Items in the current order
        /// </summary>
        public OrderItem[] Items { get; set; }
        /// <summary>
        /// To track the selected order
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
