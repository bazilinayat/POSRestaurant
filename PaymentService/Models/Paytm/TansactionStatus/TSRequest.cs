using PaymentService.Models.Paytm.CreateQR;
using System.Text.Json.Serialization;

namespace PaymentService.Models.Paytm.TansactionStatus
{
    /// <summary>
    /// To store the request we make for transaction status api
    /// </summary>
    public class TSRequest
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
        public TSRequestBody Body { get; set; }
    }
}
