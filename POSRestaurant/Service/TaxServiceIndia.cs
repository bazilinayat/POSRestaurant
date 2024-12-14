using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Service
{
    /// <summary>
    /// To calculate tax as per Indian laws
    /// </summary>
    public class TaxServiceIndia
    {
        /// <summary>
        /// Percentage amount to calculate CGST
        /// </summary>
        public decimal CGST { get; set; } = 2.5m;

        /// <summary>
        /// Percentage amount to calculate SGST
        /// </summary>
        public decimal SGST { get; set; } = 2.5m;

        /// <summary>
        /// Implementing the interface
        /// Will calcualte the tax as per the law
        /// </summary>
        /// <param name="SubTotal">Taxable Amount of bill</param>
        /// <returns>Tax percentage</returns>
        public decimal CalculateCGST(decimal SubTotal)
        {
            return (SubTotal * CGST) / 100;
        }

        /// <summary>
        /// Implementing the interface
        /// Will calcualte the tax as per the law
        /// </summary>
        /// <param name="SubTotal">Taxable Amount of bill</param>
        /// <returns>Tax percentage</returns>
        public decimal CalculateSGST(decimal SubTotal)
        {
            return (SubTotal * SGST) / 100;
        }
    }
}
