using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.CreateQR
{
    /// <summary>
    /// request body of the create api
    /// </summary>
    public class CreateRequestBody
    {
        /// <summary>
        /// merchant id of the paytm account
        /// </summary>
        [JsonPropertyName("mid")]
        public string MID { get; set; }
        /// <summary>
        /// Order id referenced by paytm
        /// </summary>
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }
        /// <summary>
        /// Amount of the order for generating qr
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Business type
        /// </summary>
        [JsonPropertyName("businessType")]
        public string BusinessType { get; set; }
        /// <summary>
        /// Unique id of the pos, to be referenced by paytm
        /// </summary>
        [JsonPropertyName("posId")]
        public string PosId { get; set; }
    }
}
