using POSRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Service.PaymentService
{
    /// <summary>
    /// Interface to define all the necessary contracts that a payment implementation must have
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// This contract should define the call to apis which give us the qrcode content in the response
        /// </summary>
        /// <param name="orderId">Order id to be showed to customer</param>
        /// <param name="orderAmount">Order amount for the qr code</param>
        /// <returns>Returns a string of qr content</returns>
        public Task<string?> GenerateDynamicQR(long orderId, decimal orderAmount);

        /// <summary>
        /// This contract should define the call to a api which gives us the status of the qr code we generated before
        /// To know if any payment was done
        /// </summary>
        /// <param name="qrId">Qr code id, to identify</param>
        /// <returns>Status of the qr code payment</returns>
        public Task<OnlinePaymentStatus> SeeQRpaymentStatus(string qrId);
    }
}
