using System;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.Model
{
    public class ProcessedMessage
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsFile{ get; set; }
        public string? FilePath { get; set; }
        public bool IsImage{ get; set; }
        public string? Text { get; set; }
        public bool IsReceivedMessage { get; set; }
        public bool IsRead { get; set; }
    }
}
