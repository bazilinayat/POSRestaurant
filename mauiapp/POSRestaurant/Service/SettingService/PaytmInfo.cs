namespace POSRestaurant.Service.SettingService
{
    /// <summary>
    /// To store the paytm information
    /// </summary>
    public class PaytmInfo
    {
        /// <summary>
        /// The URL used to craete the dynamic qr code
        /// </summary>
        public string CreateQRURL { get; set; }
        /// <summary>
        /// The URL used to check transaction status
        /// </summary>
        public string TransactionStatusURL { get; set; }
        /// <summary>
        /// The merchant ID of the paytm account
        /// </summary>
        public string MID { get; set; }
        /// <summary>
        /// The merchant key of the paytm account
        /// </summary>
        public string MerchantKey { get; set; }
        /// <summary>
        /// Unique posid of our pos system
        /// Should be unique across all payment platforms
        /// </summary>
        public string POSID { get; set; }
        /// <summary>
        /// Just a value to make the details common
        /// Could be anything we want
        /// </summary>
        public string OrderDetails { get; set; }
        /// <summary>
        /// This is code we have to pass to paytm api to generate the qr code
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// Version of the api being used
        /// </summary>
        public string Version { get; set; }
    }
}
