using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    public partial class StaffEditModel : ObservableObject
    {
        /// <summary>
        /// primary key of the waiter, to identify the records
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the waiter
        /// </summary>
        [ObservableProperty]
        private string _name;
        /// <summary>
        /// Mobile number of the waiter
        /// </summary>
        [ObservableProperty]
        private string _phoneNumber;
        /// <summary>
        /// Role of the staff member
        /// </summary>
        [ObservableProperty]
        private StaffRole _role;
    }
}
