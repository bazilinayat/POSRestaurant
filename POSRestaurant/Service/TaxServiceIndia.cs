using POSRestaurant.DBO;
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
        /// DI database service
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To know if restaurant is using GST
        /// </summary>
        public bool UsingGST { get; set; }

        /// <summary>
        /// Percentage amount to calculate CGST
        /// </summary>
        public decimal CGST { get; set; }

        /// <summary>
        /// Percentage amount to calculate SGST
        /// </summary>
        public decimal SGST { get; set; }

        public TaxServiceIndia(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            InitializeData();
        }

        /// <summary>
        /// To initialize the tax information for calculation
        /// </summary>
        /// <returns>Return a task</returns>
        public async ValueTask InitializeData()
        {
            var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

            if (restaurantInfo.UsingGST)
            {
                UsingGST = restaurantInfo.UsingGST;
                CGST = restaurantInfo.CGST;
                SGST = restaurantInfo.SGST;
            }
            else
            {
                UsingGST = false;
                CGST = SGST = 0;
            }
        }

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
