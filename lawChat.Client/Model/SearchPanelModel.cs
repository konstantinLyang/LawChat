using System;

namespace lawChat.Client.Model
{
    public class SearchPanelModel
    {
        public string Title { get; set; }
        public string LastMessage { get; set; }
        public string LasMessageDateTime { get; set; }
        public Uri ContactPhoto { get; set; }
    }
}
