using System.Windows.Input;
using LawChat.Client.Assets.CustomNotification;
using ToastNotifications.Core;

namespace LawChat.Client.View.UserControls
{
    public partial class ClientMessageDisplayPart : NotificationDisplayPart
    {
        public ClientMessageDisplayPart(ClientMessageNotification clientMessageNotification)
        {
            InitializeComponent();
            Bind(clientMessageNotification);
        }
        protected override void OnMouseEnter(MouseEventArgs e) { }
    }
}
