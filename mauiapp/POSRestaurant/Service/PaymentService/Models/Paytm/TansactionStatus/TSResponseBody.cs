using POSRestaurant.PaymentService.Models.Paytm;
using System.Text.Json.Serialization;

namespace POSRestaurant.PaymentService.Models.Paytm.TansactionStatus
{
    /// <summary>
    /// To store the response body we get from transaction status api
    /// </summary>
    public class TSResponseBody
    {
        /// <summary>
        /// Result Info of the response
        /// </summary>
        [JsonPropertyName("resultInfo")]
        public PaytmAPIResultInfo ResultInfo { get; set; }
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonPropertyName("txnId")]
        public string TxnId { get; set; }
        /// <summary>
        /// Transaction id referenced by bank
        /// </summary>
        [JsonPropertyName("bankTxnId")]
        public string BankTxnId { get; set; }
        /// <summary>
        /// order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }
        /// <summary>
        /// Transaction amount
        /// </summary>
        [JsonPropertyName("txnAmount")]
        public decimal TxnAmount { get; set; }
        /// <summary>
        /// Transaction type
        /// </summary>
        [JsonPropertyName("txnType")]
        public string TxnType { get; set; }
        /// <summary>
        /// gateway name for transaction
        /// </summary>
        [JsonPropertyName("gatewayName")]
        public string GatewayName { get; set; }
        /// <summary>
        /// Name of the bank
        /// </summary>
        [JsonPropertyName("bankName")]
        public string BankName { get; set; }
        /// <summary>
        /// Merchant id given by paytm
        /// </summary>
        [JsonPropertyName("mid")]
        public string Mid { get; set; }
        /// <summary>
        /// Payment mode
        /// </summary>
        [JsonPropertyName("paymentMode")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// Refund amount if any
        /// </summary>
        [JsonPropertyName("refundAmt")]
        public decimal RefundAmt { get; set; }
        /// <summary>
        /// Transaction date time
        /// </summary>
        [JsonPropertyName("txnDate")]
        public DateTime TxnDate { get; set; }
        /// <summary>
        /// Auth reference id
        /// </summary>
        [JsonPropertyName("authRefId")]
        public string AuthRefId { get; set; }
    }
}
