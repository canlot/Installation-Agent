using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Installation.Models;
using System.Windows.Media;
using System.Globalization;

namespace Installation_Agent.Converter
{
    public class SuccessToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is bool)
            {
                if((bool)value == true)
                    return new SolidColorBrush(Brushes.Green.Color);
                else
                    return new SolidColorBrush(Brushes.Gray.Color);
            }
            else
                return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
