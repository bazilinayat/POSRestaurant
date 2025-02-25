using POSRestaurant.PaymentService.Models.Paytm;
using POSRestaurant.PaymentService.Models.Paytm.CreateQR;
using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.TansactionStatus
{
    /// <summary>
    /// To store the response we get from transaction status api
    /// </summary>
    public class TSResponse
    {
        /// <summary>
        /// head of the response
        /// </summary>
        [JsonPropertyName("head")]
        public PaytmAPIHead Head { get; set; }
        /// <summary>
        /// body of the response
        /// </summary>
        [JsonPropertyName("body")]
        public CreateResponseBody Body { get; set; }
    }
}
