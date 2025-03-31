using System.Text.Json.Serialization;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    /// <summary>
    /// Class to represent the failure response of QR creation api for razor pay
    /// </summary>
    public class QRFailureResponse
    {
        [JsonPropertyName("error")]
        public ErrorDetails Error { get; set; }
    }

    /// <summary>
    /// Error details of the failure response
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// Error code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        /// <summary>
        /// Description of the error
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        /// <summary>
        /// Source of the error?
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
        /// <summary>
        /// Step, not sure what this is 
        /// </summary>
        [JsonPropertyName("step")]
        public string Step { get; set; }
        /// <summary>
        /// Reson for the error maybe?
        /// </summary>
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        /// <summary>
        /// Metadata for the error?
        /// </summary>
        [JsonPropertyName("metadata")]
        public object Metadata { get; set; } // Can be a dictionary or a dynamic object if needed
    }
}
