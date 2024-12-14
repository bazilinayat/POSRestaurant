using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSRestaurant.Service
{
    /// <summary>
    /// To calculate teh tax of any order given the sub total
    /// This service should be working independently
    /// Tax laws could change or different countries can use the software
    /// </summary>
    public class TaxService
    {
        /// <summary>
        /// To handle tax calculation as per Indian laws
        /// </summary>
        public TaxServiceIndia IndianTaxService;

        /// <summary>
        /// Class constructor, to generate initialize the different tax services
        /// </summary>
        public TaxService()
        {
            IndianTaxService = new TaxServiceIndia();
        }
    }
}
