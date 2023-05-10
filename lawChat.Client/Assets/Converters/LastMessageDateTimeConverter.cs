using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lawChat.Client.Assets.Converters
{
    public class LastMessageDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";

            if (value != null)
            {
                if ((DateTime)value > DateTime.Now.AddHours(-24))
                {
                    result = ((DateTime)value).ToString("HH:mm");
                }
                else if ((DateTime)value < DateTime.Now.AddHours(-24) && (DateTime)value > DateTime.Now.AddDays(-7))
                {
                    result = ((DateTime)value).ToString("ddd");
                }
                else if ((DateTime)value < DateTime.Now.AddDays(-7))
                {
                    result = ((DateTime)value).ToString("dd.MM.yy");
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
