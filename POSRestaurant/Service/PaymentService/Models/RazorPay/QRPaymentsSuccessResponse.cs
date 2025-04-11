using System.Text.Json.Serialization;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    /// <summary>
    /// Success response to get payment details for a qr code
    /// </summary>
    public class QRPaymentsSuccessResponse
    {
        /// <summary>
        /// What kind of entity are we dealing with, will be collection only
        /// </summary>
        [JsonPropertyName("entity")]
        public string Entity { get; set; }
        /// <summary>
        /// length of the collection
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
        /// <summary>
        /// Payment Items
        /// </summary>
        [JsonPropertyName("items")]
        public List<PaymentItem> Items { get; set; }
    }

    public class PaymentItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("entity")]
        public string Entity { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonPropertyName("international")]
        public bool International { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("amount_refunded")]
        public int? AmountRefunded { get; set; }

        [JsonPropertyName("refund_status")]
        public string RefundStatus { get; set; }

        [JsonPropertyName("captured")]
        public bool Captured { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("card_id")]
        public string CardId { get; set; }

        [JsonPropertyName("bank")]
        public string Bank { get; set; }

        [JsonPropertyName("wallet")]
        public string Wallet { get; set; }

        [JsonPropertyName("vpa")]
        public string Vpa { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("contact")]
        public string Contact { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("notes")]
        public Note Notes { get; set; }

        [JsonPropertyName("fee")]
        public int? Fee { get; set; }

        [JsonPropertyName("tax")]
        public int? Tax { get; set; }

        [JsonPropertyName("error_code")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error_source")]
        public string ErrorSource { get; set; }

        [JsonPropertyName("error_step")]
        public string ErrorStep { get; set; }

        [JsonPropertyName("error_reason")]
        public string ErrorReason { get; set; }

        [JsonPropertyName("acquirer_data")]
        public AcquirerData AcquirerData { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        
        [JsonPropertyName("source_channel")]
        public string SourceChannel { get; set; }

        [JsonPropertyName("upi")]
        public UPIPayment UPIPayment { get; set; }
    }

    public class AcquirerData
    {
        [JsonPropertyName("rrn")]
        public string Rrn { get; set; }
    }

    public class UPIPayment
    {
        [JsonPropertyName("payer_account_type")]
        public string PayerAccountType { get; set; }

        [JsonPropertyName("vpa")]
        public string VPA { get; set; }
    }
}
