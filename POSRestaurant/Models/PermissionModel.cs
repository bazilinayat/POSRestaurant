using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Permissions of the for users
    /// </summary>
    public partial class PermissionModel : ObservableObject
    {
        /// <summary>
        /// The id of different permissions
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the permission
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// To see if this permission is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
        /// <summary>
        /// To make object of PermissionModel
        /// </summary>
        /// <param name="entity">Permission Object</param>
        /// <returns>Returns a PermissionModel object</returns>
        public static PermissionModel FromEntity(Permission entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                IsSelected = false
            };
    }
}
