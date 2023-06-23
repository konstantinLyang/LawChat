using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace LawChat.Client.Assets.Converters
{
    public class LastMessageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";

            if (value != null)
            {
                result = Regex.Replace(value.ToString(), @"[\r\n\t]", "").Substring(0, Math.Min(Regex.Replace(value.ToString(), @"[ \r\n\t]", "").Length, 60));
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
