namespace lawChat.Server.Data
{
    public static class DbManager
    {
        public static bool RegisterNewChatMessage(int senderId, int chatId, byte[] data)
        {
            try
            {
                using (var context = new LawChatDbContext())
                {
                    context.Messages.Add(new()
                    {
                        Sender = context.Clients.FirstOrDefault(x => x.Id == senderId),
                        Chat = context.Chats.FirstOrDefault(x => x.Id == chatId),
                        Data = data
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
        public static bool RegisterNewPrivateMessage(int senderId, int recipientId, byte[] data)
        {
            try
            {
                using (var context = new LawChatDbContext())
                {
                    context.Messages.Add(new()
                    {
                        Sender = context.Clients.FirstOrDefault(x => x.Id == senderId),
                        Recipient = context.Clients.FirstOrDefault(x => x.Id == recipientId),
                        Data = data
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
