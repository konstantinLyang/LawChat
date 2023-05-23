using ToastNotifications;
using ToastNotifications.Core;

namespace LawChat.Client.Assets.CustomNotification
{
    public static class ClientMessageExtension
    {
        public static void ShowClientMessage(this Notifier notifier, string message, string senderName, MessageOptions options)
        {
            notifier.Notify<ClientMessageNotification>(() => new ClientMessageNotification(message, senderName, options));
        }
    }
}
