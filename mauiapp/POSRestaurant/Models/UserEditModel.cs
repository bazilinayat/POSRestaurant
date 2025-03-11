using CommunityToolkit.Mvvm.ComponentModel;

namespace POSRestaurant.Models
{
    /// <summary>
    /// User of the application
    /// </summary>
    public partial class UserEditModel : ObservableObject
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        [ObservableProperty]
        private string _username;
        /// <summary>
        /// Password for the user
        /// </summary>
        [ObservableProperty]
        private string _password;
        /// <summary>
        /// ID of the assigned role
        /// </summary>
        [ObservableProperty]
        private int? _assignedRoleId;
    }
}
