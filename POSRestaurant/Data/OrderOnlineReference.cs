using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Table to store the online reference for each order
    /// </summary>
    public class OrderOnlineReference
    {
        /// <summary>
        /// Unique id for the row in table
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// Id of the order, for which we are making the reference
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// Online payment reference id
        /// </summary>
        public string ReferenceId { get; set; }
        /// <summary>
        /// To reference the order payment id
        /// </summary>
        public long OrderPayemntId { get; set; }
    }
}
