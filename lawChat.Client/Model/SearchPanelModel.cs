using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using lawChat.Server.Data.Model;

namespace lawChat.Client.Model
{
    public class SearchPanelModel
    {
        public string? Title { get; set; }
        public int RecipientId { get; set; }

        public string ContactPhoto { get; set; } = null;
        public string LastMessage
        {
            get => Messages[Messages.Count - 1].Text.ToString();
        }
        public string LastMessageDateTime
        {
            get => Messages[Messages.Count - 1].CreateDate.ToString("dd.MM.yy");
        }
        public ObservableCollection<Message> Messages { get; set; }
    }
}
