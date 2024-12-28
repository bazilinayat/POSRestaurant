using CommunityToolkit.Mvvm.ComponentModel;
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
        public int Id { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        public string ExpenseTypeName { get; set; }
        /// <summary>
        /// To see if this role is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
