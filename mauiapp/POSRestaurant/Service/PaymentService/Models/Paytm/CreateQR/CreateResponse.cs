using POSRestaurant.PaymentService.Models.Paytm;
using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.CreateQR
{
    /// <summary>
    /// To store the response we get from create qr api
    /// </summary>
    public class CreateResponse
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
