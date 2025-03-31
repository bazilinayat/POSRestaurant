using System.Text.Json.Serialization;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    public class QRSuccessResponse
    {
        /// <summary>
        /// Id of the qr code created
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// Type of the request made, will be "qr_code" always
        /// Came with request
        /// </summary>
        [JsonPropertyName("entity")]
        public string Entity { get; set; }
        /// <summary>
        /// TimeStamp for created date
        /// </summary>
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        public DateTime CreatedDate
        {
            get
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(CreatedAt).ToLocalTime();
                return dateTime;
            }
        }
        /// <summary>
        /// Name of the restaurant
        /// Came with request
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// Usage of the request, will be "single_use" always
        /// Came with request
        /// </summary>
        [JsonPropertyName("usage")]
        public string Usage { get; set; }
        /// <summary>
        /// Type of request, will be "upi_qr" always
        /// Came with request
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
        /// <summary>
        /// The public url to access the image
        /// </summary>
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
        /// <summary>
        /// The payment amount to be paid by customer
        /// Came with request
        /// </summary>
        [JsonPropertyName("payment_amount")]
        public int PaymentAmount { get; set; }
        /// <summary>
        /// The status of payment or qr code
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
        /// <summary>
        /// Description for the payment
        /// Came with request
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        /// <summary>
        /// To know if customer can change the amount to be paid
        /// Came with request
        /// </summary>
        [JsonPropertyName("fixed_amount")]
        public bool FixedAmount { get; set; }
        /// <summary>
        /// To know how much payment amount was received
        /// </summary>
        [JsonPropertyName("payments_amount_received")]
        public int PaymentsAmountReceived { get; set; }
        /// <summary>
        /// To know how many times the payment was done on the qr code
        /// </summary>
        [JsonPropertyName("payments_count_received")]
        public int PaymentsCountReceived { get; set; }
        /// <summary>
        /// Notes for the transaction
        /// Came with request
        /// </summary>
        [JsonPropertyName("notes")]
        public Note Notes { get; set; }
        /// <summary>
        /// Customer Id
        /// Came with request
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }
        /// <summary>
        /// The TTL for the QR Code
        /// </summary>
        [JsonPropertyName("close_by")]
        public long CloseBy { get; set; }
    }

}
