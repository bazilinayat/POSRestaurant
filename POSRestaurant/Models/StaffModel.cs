using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Staff Model to be used on ui
    /// </summary>
    public partial class StaffModel : ObservableObject
    {
        /// <summary>
        /// primary key of the waiter, to identify the records
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the waiter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mobile number of the waiter
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Role of the staff member
        /// </summary>
        public StaffRole Role { get; set; }
        /// <summary>
        /// To track the selected order
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
