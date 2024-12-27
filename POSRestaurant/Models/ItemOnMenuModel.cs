using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Model for ItemOnMenu for editing purposes
    /// </summary>
    public partial class ItemOnMenuModel : ObservableObject
    {
        /// <summary>
        /// Id to identify the item correctly
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the item
        /// Observable to be changed
        /// </summary>
        [ObservableProperty]
        private string _name;
        /// <summary>
        /// Price of the item
        /// Observable to be changed
        /// </summary>
        [ObservableProperty]
        private decimal _price;
        /// <summary>
        /// Description of the item
        /// Observable to be changed
        /// </summary>
        [ObservableProperty]
        private string _description;
        /// <summary>
        /// Category this item belongs to
        /// </summary>
        [ObservableProperty]
        private MenuCategoryModel _category;
        /// <summary>
        /// Shortcode for the menu item for quick search
        /// </summary>
        [ObservableProperty]
        private string _shortCode;
    }
}
