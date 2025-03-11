using POSRestaurant.PaymentService.Models.Paytm;
using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.CreateQR
{
    /// <summary>
    /// To store the response body we get from create qr api
    /// </summary>
    public class CreateResponseBody
    {
        /// <summary>
        /// Result Info of the response
        /// </summary>
        [JsonPropertyName("resultInfo")]
        public PaytmAPIResultInfo ResultInfo { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("qrCodeId")]
        public string QrCodeId { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("qrData")]
        public string QrData { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}
