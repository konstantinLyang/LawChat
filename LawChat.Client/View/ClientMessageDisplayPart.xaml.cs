using LawChat.Client.Assets.CustomNotification;
using ToastNotifications.Core;

namespace LawChat.Client.View
{
    public partial class ClientMessageDisplayPart : NotificationDisplayPart
    {
        public ClientMessageDisplayPart(ClientMessageNotification clientMessageNotification)
        {
            InitializeComponent();
            Bind(clientMessageNotification);
        }
    }
}
