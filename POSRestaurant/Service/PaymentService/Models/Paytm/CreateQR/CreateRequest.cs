using POSRestaurant.PaymentService.Models.Paytm;
using POSRestaurant.PaymentService.Models.Paytm;
using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.CreateQR
{
    /// <summary>
    /// To store the request we make for create qr api
    /// </summary>
    internal class CreateRequest
    {
        /// <summary>
        /// head of the request
        /// </summary>
        [JsonPropertyName("head")]
        public PaytmAPIHead Head { get; set; }
        /// <summary>
        /// body of the request
        /// </summary>
        [JsonPropertyName("body")]
        public CreateRequestBody Body { get; set; }
    }
}
