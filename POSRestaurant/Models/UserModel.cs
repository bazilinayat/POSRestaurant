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
    /// User of the application
    /// </summary>
    public partial class UserModel : ObservableObject
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password for the user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// ID of the assigned role
        /// </summary>
        public int AssignedRoleId { get; set; }
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
        public static UserModel FromEntity(User entity) =>
            new()
            {
                Id = entity.Id,
                Username = entity.Username,
                AssignedRoleId = 0,
                IsSelected = false
            };
    }
}
