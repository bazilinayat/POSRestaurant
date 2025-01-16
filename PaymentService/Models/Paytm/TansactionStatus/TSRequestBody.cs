using System.Text.Json.Serialization;

namespace PaymentService.Models.Paytm.TansactionStatus
{
    /// <summary>
    /// request body of the transaction status api
    /// </summary>
    public class TSRequestBody
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
    }
}
