using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PaymentService.Models.Paytm.CreateQR
{
    /// <summary>
    /// To store the request we make for create qr api
    /// </summary>
    internal class CreateRequest
    {
        /// <summary>
        /// head of the request
        /// </summary>
        [JsonPropertyName("head")]
        public PaytmAPIHead Head { get; set; }
        /// <summary>
        /// body of the request
        /// </summary>
        [JsonPropertyName("body")]
        public CreateRequestBody Body { get; set; }
    }
}
