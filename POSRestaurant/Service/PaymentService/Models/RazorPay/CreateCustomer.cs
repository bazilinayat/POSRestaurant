using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POSRestaurant.Service.PaymentService.Models.RazorPay
{
    /// <summary>
    /// Model to create a customer
    /// </summary>
    public class CreateCustomer
    {
        /// <summary>
        /// Name of the customer
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// Email of the customer
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }
        /// <summary>
        /// Contact of the customer
        /// </summary>
        [JsonPropertyName("contact")]
        public string Contact { get; set; }
        /// <summary>
        /// To fail the creation when the same customer exists
        /// </summary>
        [JsonPropertyName("fail_existing")]
        public string FailExisting { get; set; }
        /// <summary>
        /// gstin of the customer
        /// </summary>
        [JsonPropertyName("gstin")]
        public string GSTIn { get; set; } = "12ABCDE3456F7GH";
        /// <summary>
        /// Normally will not be included in the request, but just keeping here
        /// </summary>
        [JsonPropertyName("notes")]
        public Note Notes { get; set; }
    }
}
