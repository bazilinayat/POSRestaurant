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
        /// To store the GST number of the business
        /// </summary>
        public string GSTIN { get; set; }
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
    }
}
