using POSRestaurant.Service;
using System.Globalization;

namespace POSRestaurant.Converters
{
    public class PermissionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string permissionName = parameter.ToString();
            IAuthService authService = App.Current.Handler.MauiContext.Services.GetService<IAuthService>();

            return authService?.HasPermission(permissionName) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
