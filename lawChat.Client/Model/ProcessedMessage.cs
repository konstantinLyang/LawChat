using System;
using System.Windows.Input;
using lawChat.Client.Infrastructure;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.Model
{
    public class ProcessedMessage : ViewModelBase
    {
        public int Id { get; set; }
        public int FileId { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsFile { get; set; }

        public string? FilePath { get; set; } = "";

        public string? ServerFilePath { get; set; }

        public bool IsImage{ get; set; }

        public string? Text { get; set; }

        public bool IsReceivedMessage { get; set; }

        public bool IsRead { get; set; }

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get => _openFileCommand;
            set => _openFileCommand = value;
        }
    }
}
