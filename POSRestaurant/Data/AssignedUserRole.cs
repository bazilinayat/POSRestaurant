namespace POSRestaurant.Data
{
    /// <summary>
    /// To store the permissions which are for the roles
    /// </summary>
    public class AssignedUserRole
    {
        /// <summary>
        /// The user id 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The role id assigned to user
        /// </summary>
        public int RoleId { get; set; }
    }
}
