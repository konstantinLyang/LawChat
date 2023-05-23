using System.ComponentModel;
using System.Runtime.CompilerServices;
using LawChat.Client.View;
using ToastNotifications.Core;

namespace LawChat.Client.Assets.CustomNotification
{
    public class ClientMessageNotification : NotificationBase, INotifyPropertyChanged
    {
        private ClientMessageDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new ClientMessageDisplayPart(this));

        public ClientMessageNotification(string message, string senderName, MessageOptions options) : base(message, options)
        {
            Message = message;
            UserName = senderName;
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
