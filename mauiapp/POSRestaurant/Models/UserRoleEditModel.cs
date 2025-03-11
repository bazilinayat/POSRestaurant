using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// To edit th role on ui
    /// </summary>
    public partial class UserRoleEditModel : ObservableObject
    {
        /// <summary>
        /// primary key of the role, to identify the records
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the role
        /// </summary>
        [ObservableProperty]
        private string _name;
        /// <summary>
        /// Permissions of the role
        /// </summary>
        [ObservableProperty]
        private List<PermissionModel> _permissions;
    }
}
