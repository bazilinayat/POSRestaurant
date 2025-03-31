using System.Text.Json.Serialization;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    /// <summary>
    /// To send notes in creation request
    /// </summary>
    public class Note
    {
        /// <summary>
        /// If you want to send the purpose of the payment
        /// </summary>
        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }
    }
}
