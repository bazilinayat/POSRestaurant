using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class RoleModel : ObservableObject
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// To see if this role is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
