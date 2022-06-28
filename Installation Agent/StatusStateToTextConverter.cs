﻿using Installation.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Installation_Agent
{
    public class StatusStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Log.Verbose("Convert value from StatusStateToTextConverter");
            if (value == null) return null;
            if (value is StatusState)
            {
                Log.Verbose("Value is StatusState {value}", (StatusState)value);
                switch (value)
                {
                    case StatusState.Success:
                        return "Erfogreich ausgeführt";
                    case StatusState.Warning:
                        return "Ausgeführt mit Warnungen";
                    case StatusState.Error:
                        return "Fehler beim Ausführen aufgetreten";
                    default:
                        return "";
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
