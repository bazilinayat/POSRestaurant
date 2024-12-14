using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// To represent the Indian tax model
    /// </summary>
    public class IndianTaxModel
    {
        /// <summary>
        /// CGST Amount of the order
        /// </summary>
        public decimal CGST { get; set; }

        /// <summary>
        /// SGST Amount of the order
        /// </summary>
        public decimal SGST { get; set; }

        /// <summary>
        /// CGST Amount of the order
        /// </summary>
        public decimal CGSTAmount { get; set; }

        /// <summary>
        /// SGST Amount of the order
        /// </summary>
        public decimal SGSTAmount { get; set; }
    }
}
