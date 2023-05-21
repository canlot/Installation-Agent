using Installation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Installation_Agent
{
    public class ExecutionStateToIconColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return null;
            if(value is ExecutableBase)
            {
                if(value is ApplicationExecutable)
                {
                    ApplicationExecutable applicationExecutable = (ApplicationExecutable)value;
                    if((applicationExecutable.Installed || applicationExecutable.ReInstalled) && !applicationExecutable.UnInstalled)
                        return new SolidColorBrush(Brushes.Green.Color);
                    else
                        return new SolidColorBrush(Brushes.Gray.Color);
                }
                else if(value is ScriptExecutable)
                {
                    ScriptExecutable scriptExecutable = (ScriptExecutable)value;
                    if (scriptExecutable.Runned)
                        return new SolidColorBrush(Brushes.Green.Color);
                    else
                        return new SolidColorBrush(Brushes.Gray.Color);

                }

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
