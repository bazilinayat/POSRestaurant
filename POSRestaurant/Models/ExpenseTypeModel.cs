using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// To represent the expense types
    /// </summary>
    public partial class ExpenseTypeModel : ObservableObject
    {
        /// <summary>
        /// Id of the role
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// To see if this role is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        public static ExpenseTypeModel FromEntity(ExpenseTypes entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                IsSelected = false
            };
    }
}
