using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Generic model for whenever a text is suppose to be selected from a list
    /// </summary>
    public partial class TextSelectModel : ObservableObject
    {
        public string Text { get; set; }
        /// <summary>
        /// Observable Property to see if Text is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}
