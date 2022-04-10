using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Installation.Models;
using System.Windows.Media;
using Serilog;

namespace Installation_Agent
{
    public class StatusStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Log.Debug("Convert value");
            if (value == null) return null;
            if(value is StatusState)
            {
                Log.Debug("Value is StatusState {value}", (StatusState)value);
                switch (value)
                {
                    case StatusState.NotExecuted:
                        return new SolidColorBrush(Brushes.Azure.Color);
                    case StatusState.Success:
                        return new SolidColorBrush(Brushes.Green.Color);
                    case StatusState.Warning:
                        return new SolidColorBrush(Brushes.Orange.Color);
                    case StatusState.Error:
                        return new SolidColorBrush(Brushes.Red.Color);
                    default:
                        return new SolidColorBrush(Brushes.Azure.Color);
                }
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
