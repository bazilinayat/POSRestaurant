using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Model for ExpsenseItem purposes
    /// </summary>
    public partial class ExpenseItemModel : ObservableObject
    {
        /// <summary>
        /// Primary id of the items
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item type
        /// </summary>
        public ExpenseItemTypes ItemType { get; set; }

        /// <summary>
        /// True if item is to be weighted
        /// False if quntity
        /// </summary>
        public bool IsWeighted { get; set; }

        /// <summary>
        /// To track the selected order
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// To make object of ExpenseItemModel
        /// </summary>
        /// <param name="entity">ExpenseItem Object</param>
        /// <returns>Returns a ExpenseItemModel object</returns>
        public static ExpenseItemModel FromEntity(ExpenseItem entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                ItemType = entity.ItemType,
                IsWeighted = entity.IsWeighted,
            };
    }
}
