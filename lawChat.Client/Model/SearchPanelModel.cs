using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataBase.Data.Model;

namespace lawChat.Client.Model
{
    public class SearchPanelModel
    {
        public string? Title { get; set; }
        public int RecipientId { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
    }
}
