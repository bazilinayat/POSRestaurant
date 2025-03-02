using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Role to be assigned to each user
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Name of the role
        /// </summary>
        [Unique]
        public string Name { get; set; }
        /// <summary>
        /// Different permissions assigned to this role
        /// </summary>
        [Ignore]
        public Permission[] Permissions { get; set; }
    }
}
