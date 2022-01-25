using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DarwinBots.Forms
{
    internal class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsEnum)
                return (int)value;

            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int val && targetType.IsEnum && Enum.IsDefined(targetType, val))
            {
                return Enum.ToObject(targetType, val);
            }

            throw new InvalidOperationException();
        }
    }
}
