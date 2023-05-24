using LawChat.Client.Model.Enums;

namespace LawChat.Client.Model
{
    public class StickerModel
    {
        public string Name { get; set; }
        public EmojiType Type { get; set; }
        public string ImageFilePath { get; set; }
    }
}
