using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TestingClientApp
{
    internal class TimeToTimeTimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
            {
                if ((int)time.TotalHours <= 0)
                {
                    //return duraion.ToString("mm':'ss");
                    return time.ToString("mm\\:ss");
                }
                else
                {
                    return time.ToString("h\\:mm\\:ss");
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
