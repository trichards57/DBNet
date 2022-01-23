using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DarwinBots.Forms
{
    internal class NumberScaleConverter : IValueConverter
    {
        public int Scaler { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double val)
                return (val * Math.Pow(10, Scaler)).ToString("F");

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string valStr)
                return DependencyProperty.UnsetValue;

            var valValid = double.TryParse(valStr, out var val);

            if (!valValid)
                return DependencyProperty.UnsetValue;

            return val * Math.Pow(10, -Scaler);
        }
    }
}
