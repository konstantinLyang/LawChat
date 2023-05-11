using System;

namespace lawChat.Client.Model
{
    public class ProcessedMessage
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Text { get; set; }
        public bool IsReceivedMessage { get; set; }
    }
}
