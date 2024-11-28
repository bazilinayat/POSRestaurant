using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Service
{
    /// <summary>
    /// PrintingService to list down methods related to printing
    /// </summary>
    public interface IReceiptPrinterService
    {
        /// <summary>
        /// Generic method to be implemented for different operation systems
        /// </summary>
        /// <param name="receiptData"></param>
        /// <returns></returns>
        void PrintAsync(string receiptData);
    }
}
