using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Converters
{
    /// <summary>
    /// Converter to convert the integer value to boolean for UI use
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Convert the given integer value to boolean
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Integer value output</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">CulterInfo for UI</param>
        /// <returns>Returns boolean in case of success, else false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedValue && parameter is string targetValue)
            {
                return selectedValue == int.Parse(targetValue);
            }
            return false;
        }

        /// <summary>
        /// Convert the given boolean value to integer
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Integer value output</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">CulterInfo for UI</param>
        /// <returns>Returns number in case of success, else 0</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string targetValue)
            {
                return int.Parse(targetValue);
            }
            return 0;
        }
    }
}
