using System;
using System.Globalization;
using System.Windows.Data;

namespace GarminAnalyzer.Util
{
    public class DisplayFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case int _:
                    return $"{value} {parameter}";
                case double val:
                    return $"{Math.Round(val)} {parameter}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}