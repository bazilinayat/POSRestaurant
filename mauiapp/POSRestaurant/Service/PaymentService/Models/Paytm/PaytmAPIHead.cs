using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm
{
    /// <summary>
    /// To represent the head part of the api request and resposne
    /// </summary>
    public class PaytmAPIHead
    {
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("responseTimestamp")]
        public DateTime ResponseTimeStamp { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
