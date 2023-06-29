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
                result = Regex.Replace(value.ToString(), @"[\r\n\t]", "");

                if (result.Length > 30)
                {
                    return result.Substring(30, result.Length - 30);
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
