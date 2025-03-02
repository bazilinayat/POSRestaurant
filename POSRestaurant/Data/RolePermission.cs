namespace POSRestaurant.Data
{
    /// <summary>
    /// To store the permissions which are for the roles
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// The role id for the different permissions
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// The permission id for the different roles
        /// </summary>
        public int PermissionId { get; set; }
    }
}
