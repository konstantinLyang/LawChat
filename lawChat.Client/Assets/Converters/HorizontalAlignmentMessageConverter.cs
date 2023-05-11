using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using lawChat.Client.Services;

namespace lawChat.Client.Assets.Converters
{
    public class HorizontalAlignmentMessageConverter : IValueConverter
    {
        private IClientData _clientData;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextAlignment textAlignment = TextAlignment.Left;
            try
            {
                if (parameter != null)
                {
                    if (_clientData.UserData != null && (int)parameter == _clientData.UserData.Id) textAlignment = TextAlignment.Right;
                    else textAlignment = TextAlignment.Left;
                }

                return textAlignment;
            }
            catch {return textAlignment;}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public HorizontalAlignmentMessageConverter(IClientData clientData)
        {
            _clientData = clientData;
        }
    }
}
