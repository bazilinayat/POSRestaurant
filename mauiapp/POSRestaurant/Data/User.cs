using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// User of the application, used for login
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Username for the user, should be unique
        /// </summary>
        [Unique]
        public string Username { get; set; }
        /// <summary>
        /// Password for the user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Different roles assigned to this user
        /// </summary>
        [Ignore]
        public UserRole[] Roles { get; set; }
    }
}
