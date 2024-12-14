using POSRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Converters
{
    /// <summary>
    /// To convert the payment mode enum to boolean and back
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Convert the given Enum value to boolean
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Boolean</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture of application</param>
        /// <returns>Returns value, else false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PaymentModes selectedMode && parameter is PaymentModes buttonMode)
            {
                return selectedMode == buttonMode;
            }
            return false;
        }

        /// <summary>
        /// Convert the given boolean value to Enum
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Enum</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture of application</param>
        /// <returns>Returns Enum value, else null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is PaymentModes buttonMode)
            {
                return buttonMode;
            }
            return null;
        }
    }
}
