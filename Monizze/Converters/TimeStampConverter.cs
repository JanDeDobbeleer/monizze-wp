using System;
using Windows.UI.Xaml.Data;

namespace Monizze.Converters
{
    public class TimeStampConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                long tick;
                if(!long.TryParse(value.ToString(), out tick))
                    return DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                return FromUnixTime(tick).ToString("dd/MM/yyyy HH:mm");
            }
            catch (Exception)
            {
                return DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            }
        }


        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
