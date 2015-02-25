using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Monizze.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var bo = value as bool?;
                if (bo == null)
                    return Visibility.Collapsed;
                if (parameter == null)
                    return ((bool)bo) ? Visibility.Visible : Visibility.Collapsed;
                return (!(bool)bo) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var vis = value as Visibility?;
            if (vis == null)
                return false;
            return ((Visibility)vis == Visibility.Visible);
        }
    }
}
