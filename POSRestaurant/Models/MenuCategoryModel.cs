using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    /// <summary>
    /// MenuCategory Model to be observed
    /// </summary>
    public partial class MenuCategoryModel : ObservableObject
    {
        /// <summary>
        /// Menu Category Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Menu Category Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Menu Category Icon Name
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Observable Property to see if MenuCategory is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// To make object of MenuCategoryModel
        /// </summary>
        /// <param name="entity">MenuCategory Object</param>
        /// <returns>Returns a MenuCategoryModel object</returns>
        public static MenuCategoryModel FromEntity(MenuCategory entity) =>
            new ()
            {
                Id = entity.Id,
                Name = entity.Name,
                Icon = entity.Icon
            };
    }
}
