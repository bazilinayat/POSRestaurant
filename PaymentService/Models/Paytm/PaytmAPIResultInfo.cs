using System.Text.Json.Serialization;

namespace PaymentService.Models.Paytm
{
    /// <summary>
    /// To represent the result info part of the api resposne
    /// </summary>
    public class PaytmAPIResultInfo
    {
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("resultStatus")]
        public string ResultStatus { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("resultCode")]
        public string ResultCode { get; set; }
        /// <summary>
        /// time stamp of the response
        /// </summary>
        [JsonPropertyName("resultMsg")]
        public string ResultMsg { get; set; }
    }
}
