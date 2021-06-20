﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DarwinBots.Forms
{
    internal class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return !val;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return !val;
            return true;
        }
    }
}
