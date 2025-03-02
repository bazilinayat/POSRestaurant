using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Different permissions of the application to be assigned to roles
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// The id of different permissions
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Name of the permission
        /// </summary>
        [Unique]
        public string Name { get; set; }
        /// <summary>
        /// To make object of Permission
        /// </summary>
        /// <param name="entity">PermissionModel Object</param>
        /// <returns>Returns a Permission object</returns>
        public static Permission FromEntity(PermissionModel entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name
            };
    }
}
