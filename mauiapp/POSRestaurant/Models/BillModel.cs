using POSRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    public class BillModel
    {
        public string RestrauntName { get; set; }
        public string Address { get; set; }
        public string GSTIn { get; set; }
        public string CustomerName { get; set; }
        public DateTime TimeStamp { get; set; }
        public int TableNo { get; set; }
        public string Cashier { get; set; }
        public string BillNo { get; set; }
        public string TokenNos { get; set; }
        public OrderTypes OrderType { get; set; }
        public string WaiterAssigned { get; set; }
        public List<KOTItemBillModel> Items { get; set; }
        public int TotalQty { get; set; }
        public decimal SubTotal { get; set; }

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

        public decimal SubTotalAfterDiscount { get; set; }
        public bool UsginGST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal GrandTotal { get; set; }
        public string FassaiNo { get; set; }
        public string QRCode { get; set; }

        public string Source { get; set; }
        public string ReferenceNo { get; set; }
        public string DeliveryPersonName { get; set; }
    }
}
