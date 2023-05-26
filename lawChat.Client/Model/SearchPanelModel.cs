using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lawChat.Client.Model
{
    public class SearchPanelModel : INotifyPropertyChanged
    {
        public string? Title { get; set; }

        private bool? _isOnline = false; 
        public bool? IsOnline
        {
            get => _isOnline;
            set => Set(ref _isOnline, value);
        }

        private bool? _isRead = true; 
        public bool? IsRead
        {
            get => _isRead;
            set => Set(ref _isRead, value);
        }

        public int RecipientId { get; set; }

        public string ContactPhoto { get; set; } = null;

        private string? _lastMessage;
        public string? LastMessage
        {
            get => _lastMessage;
            set => Set(ref _lastMessage, value);
        }

        private DateTime? _lastMessageDateTime;
        public DateTime? LastMessageDateTime
        {
            get => _lastMessageDateTime;
            set => Set(ref _lastMessageDateTime, value);
        }

        public ObservableCollection<ProcessedMessage> Messages { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
