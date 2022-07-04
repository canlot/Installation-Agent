using Installation.Models;
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
    public class RunStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is RunState)
            {
                RunState state = (RunState)value;
                if (state == RunState.Runned)
                    return new SolidColorBrush(Brushes.Green.Color);
                else
                    return new SolidColorBrush(Brushes.Gray.Color);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
