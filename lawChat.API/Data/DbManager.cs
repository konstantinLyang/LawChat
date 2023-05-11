using lawChat.API.Data;

namespace DataBase.Data
{
    public static class DbManager
    {
        public static bool RegisterNewChatMessage(int senderId, int chatId, string message)
        {
            try
            {
                using (var context = new LawChatDbContext())
                {
                    context.Messages.Add(new()
                    {
                        Sender = context.Clients.FirstOrDefault(x => x.Id == senderId),
                        Chat = context.Chats.FirstOrDefault(x => x.Id == chatId),
                        Text = message
                    });

                    context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool RegisterNewPrivateMessage(int senderId, int recipientId, string message)
        {
            try
            {
                using (var context = new LawChatDbContext())
                {
                    context.Messages.Add(new()
                    {
                        Sender = context.Clients.FirstOrDefault(x => x.Id == senderId),
                        Recipient = context.Clients.FirstOrDefault(x => x.Id == recipientId),
                        Text = message
                    });

                    context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
