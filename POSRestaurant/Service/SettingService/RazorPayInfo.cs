namespace POSRestaurant.Service.SettingService
{
    /// <summary>
    /// To store the RazorPay information
    /// </summary>
    public class RazorPayInfo
    {
        /// <summary>
        /// The URL used to craete the dynamic qr code
        /// </summary>
        public string CreateQRURL { get; set; }
        /// <summary>
        /// The URL used to fetch the payment status of qr code
        /// </summary>
        public string FetchPaymentURL { get; set; }
        /// <summary>
        /// The merchant id of the RazorPay account
        /// </summary>
        public string KEYID { get; set; }
        /// <summary>
        /// The merchant secret of the RazorPay account
        /// </summary>
        public string KEYSECRET { get; set; }
    }
}
