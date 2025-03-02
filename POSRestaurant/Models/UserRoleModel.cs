using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Roles of the staff members
    /// </summary>
    public partial class UserRoleModel : ObservableObject
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Permissions of the role
        /// </summary>
        public List<PermissionModel> Permissions { get; set; }
        /// <summary>
        /// To see if this role is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
        /// <summary>
        /// To make object of UserRoleModel
        /// </summary>
        /// <param name="entity">Role Object</param>
        /// <returns>Returns a UserRoleModel object</returns>
        public static UserRoleModel FromEntity(UserRole entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Permissions = new List<PermissionModel>(),
                IsSelected = false
            };
    }
}
