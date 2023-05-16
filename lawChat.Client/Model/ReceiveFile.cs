using System.IO;

namespace lawChat.Client.Model
{
    public class FileMessage
    {
        public byte[] Data;
        public string FileName { get; set; }
        public FileInfo File{ get; set; }
    }
}
