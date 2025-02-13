using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// To represent the RestaurantInfo table in database
    /// </summary>
    public class RestaurantInfo
    {
        /// <summary>
        /// OrderItem Id
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// To store the food license registration id
        /// </summary>
        public string FSSAI { get; set; }
        /// <summary>
        /// To store the restaurant name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// To store teh Address of the restaurant
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// To store the phone number of the restaurant
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// To know if the restaurant is using GST
        /// </summary>
        public bool UsingGST { get; set; }
        /// <summary>
        /// To store the GST number of the business
        /// </summary>
        public string GSTIN { get; set; }
        /// <summary>
        /// If you are using GST, then what is the percent for cgst
        /// </summary>
        public decimal CGST { get; set; }
        /// <summary>
        /// If you are using GST, then what is the percent for sgst
        /// </summary>
        public decimal SGST { get; set; }
    }
}
