using System.Text.Json.Serialization;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    /// <summary>
    /// class to create request object to genearte dynamic qr code
    /// </summary>
    public class CreateQR
    {
        /// <summary>
        /// The type of the request will be upi_qr always
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; } = "upi_qr";
        /// <summary>
        /// The name of the restaurant
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// The usage of the qr code will always be single
        /// </summary>
        [JsonPropertyName("usage")]
        public string Usage { get; } = "single_use";
        /// <summary>
        /// The fixed amount type will always be true
        /// </summary>
        [JsonPropertyName("fixed_amount")]
        public bool FixedAmount { get; } = true;
        /// <summary>
        /// The amount to be paid
        /// </summary>
        [JsonPropertyName("payment_amount")]
        public double PaymentAmount { get; set; }
        /// <summary>
        /// The description for the qr code payment
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        /// <summary>
        /// The customer id who is going to pay
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = "cust_QAUZNzF1sMVYHl";
        /// <summary>
        /// The lifetime of the qr code, will always be 5 minutes  
        /// </summary>
        [JsonPropertyName("close_by")]
        public long CloseBy { get { return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (5 * 60); } }
        /// <summary>
        /// Normally will not be included in the request, but just keeping here
        /// </summary>
        [JsonPropertyName("notes")]
        public Note Notes { get; set; }
    }
}
