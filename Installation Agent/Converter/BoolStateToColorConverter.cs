using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Installation_Agent.Converter
{
    public class BoolStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Brushes.Gray.Color);
            if (value is bool)
            {
                if((bool)value == true)
                    return new SolidColorBrush(Brushes.Green.Color);
                else
                    return new SolidColorBrush(Brushes.Red.Color);
            }
            else
                return new SolidColorBrush(Brushes.Gray.Color);

        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
