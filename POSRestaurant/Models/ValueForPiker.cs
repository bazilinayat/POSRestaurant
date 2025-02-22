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
    /// This class will be used to make lists of things to populate a picker control
    /// Later we can use the key value here for identifying records
    /// </summary>
    public class ValueForPicker
    {
        /// <summary>
        /// Key of the value, to identify the record
        /// </summary>
        public int Key {  get; set; }

        /// <summary>
        /// Text to be displayed on picker
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// This class will be used to make lists of things to populate a picker control
    /// Later we can use the key value here for identifying records
    /// </summary>
    public partial class ValueForPickerSelection : ObservableObject
    {
        /// <summary>
        /// Key of the value, to identify the record
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Text to be displayed on picker
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Observable Property to see if MenuCategory is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        public static ValueForPickerSelection FromEntity(ValueForPicker entity) =>
            new()
            {
                Key = entity.Key,
                Value = entity.Value,
                IsSelected = false
            };
    }
}
