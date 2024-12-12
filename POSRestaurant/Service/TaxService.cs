using System;
using System.Collections.Generic;
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
        /// To see if settings is initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Tax settings to be taken from DB
        /// </summary>
        //public readonly TaxModel TaxToApply;

        /// <summary>
        /// Constructor to set settings object
        /// </summary>
        public decimal SettingService(decimal subTotal)
        {
            decimal totalAfterTax = 0;

            return totalAfterTax;
        }
    }
}
