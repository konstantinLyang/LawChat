using System.ComponentModel;
using System.Runtime.CompilerServices;
using LawChat.Client.View.UserControls;
using ToastNotifications.Core;

namespace LawChat.Client.Assets.CustomNotification
{
    public class ClientMessageNotification : NotificationBase, INotifyPropertyChanged
    {
        private ClientMessageDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ??= new ClientMessageDisplayPart(this);

        public ClientMessageNotification(string message, string senderName, string senderPhoto, MessageOptions options) : base(message, options)
        {
            Message = message;
            UserName = senderName;
            SenderPhoto = senderPhoto;
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _senderPhoto;
        public string SenderPhoto
        {
            get => _senderPhoto;
            set
            {
                _senderPhoto = value;
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
