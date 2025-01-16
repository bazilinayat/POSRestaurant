namespace SettingLibrary
{
    /// <summary>
    /// Class to keep the values taken from Settings.json file
    /// Initialized in SettingsViewModel
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Name of the Customer
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// To store dfault tax percentage
        /// </summary>
        public decimal DefaultTaxPercentage { get; set; }
        /// <summary>
        /// phone number of the developer
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email of the developer
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Website to redirect to, when developer name is clicked
        /// </summary>
        public string WebsiteURL { get; set; }
        /// <summary>
        /// To set the number of tables in the restaurant
        /// </summary>
        public int NumberOfTables { get; set; }
        /// <summary>
        /// To get the paytm info needed for the paytm transactions
        /// </summary>
        public PaytmInfo PaytmInfo { get; set; }
    }
}
