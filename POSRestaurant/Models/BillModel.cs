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
        public Discount Discount { get; set; }
        public decimal SubTotalAfterDiscount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal GrandTotal { get; set; }
        public string FassaiNo { get; set; }
        public string QRCode { get; set; }
    }
}
