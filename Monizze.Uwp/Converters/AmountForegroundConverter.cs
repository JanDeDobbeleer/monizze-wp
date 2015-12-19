using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Monizze.Converters
{
    public class AmountForegroundConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return (value.ToString().StartsWith("-"))
                    ? (SolidColorBrush) Application.Current.Resources["MonizzeOrange"]
                    : (SolidColorBrush)Application.Current.Resources["MonizzeLightBlue"];
            }
            catch (Exception)
            {
                return (SolidColorBrush)Application.Current.Resources["MonizzeOrange"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
