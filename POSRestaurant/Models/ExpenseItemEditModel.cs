using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using SQLite;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Model for ExpsenseItem for editing purposes
    /// </summary>
    public partial class ExpenseItemEditModel : ObservableObject
    {
        /// <summary>
        /// Primary id of the items
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Item type
        /// </summary>
        [ObservableProperty]
        private ExpenseItemTypes _itemType;

        /// <summary>
        /// True if item is to be weighted
        /// False if quntity
        /// </summary>
        [ObservableProperty]
        private bool _isWeighted;
    }
}
