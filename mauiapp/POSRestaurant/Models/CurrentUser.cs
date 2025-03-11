namespace POSRestaurant.Models
{
    /// <summary>
    /// This will help us keep track of the current user
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// Current user id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Current user name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Roles assigned to current user
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
        /// <summary>
        /// Permissions assigned to current user
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
