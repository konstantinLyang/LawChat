using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private IClientObject _clientObject;

        private ObservableCollection<SearchPanelModel> _searchPanelSource = new()
        {
            new SearchPanelModel()
            {
                Title = "Алексей",
                LastMessage = "ad  lasjdlj alsdjl aslkdj aslkdjaslkdjaslkdjaslkdjaslkdjaslkdjaslkdjaslkdj",
                LasMessageDateTime = "08:32",
                ContactPhoto = new Uri(@"D:\User\VisualStudio\lawChat\lawChat\lawChat.Client\Assets\Image\contactPhotoTest.JPG", UriKind.RelativeOrAbsolute)
            },
            new SearchPanelModel()
            {
                Title = "Валентин",
                LastMessage = "ad  lasjdlj alsdjl aslkdj",
                LasMessageDateTime = "08:32",
                ContactPhoto = new Uri(@"D:\User\VisualStudio\lawChat\lawChat\lawChat.Client\Assets\Image\contactPhotoTest.JPG", UriKind.RelativeOrAbsolute)
            },
            new SearchPanelModel()
            {
                Title = "Сергей",
                LastMessage = "ad  lasjdlj alsdjl aslkdj",
                LasMessageDateTime = "08:32",
                ContactPhoto = new Uri(@"D:\User\VisualStudio\lawChat\lawChat\lawChat.Client\Assets\Image\contactPhotoTest.JPG", UriKind.RelativeOrAbsolute)
            }
        };

        public ObservableCollection<SearchPanelModel> SearchPanelSource
        {
            get => _searchPanelSource;
            set => Set(ref _searchPanelSource, value);
        }

        private string _currentChatName = "Общий чат";
        public string CurrentChatName 
        {
            get => _currentChatName;
            set => Set(ref _currentChatName, value);
        }

        private int _currentChatId = 0;
        public int CurrentChatId
        {
            get => _currentChatId;
            set => Set(ref _currentChatId, value);
        }

        private string _currentChatTextBox;
        public string CurrentChatTextBox
        {
            get => _currentChatTextBox;
            set => Set(ref _currentChatTextBox, value);
        }

        private string _currentMessageTextBox;
        public string CurrentMessageTextBox
        {
            get => _currentMessageTextBox;
            set => Set(ref _currentMessageTextBox, value);
        }

        private LambdaCommand _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand ??= new(OnSendMessageCommand);
        private void OnSendMessageCommand()
        {
            Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrEmpty(CurrentMessageTextBox))
                {
                    var tempMessageText = CurrentMessageTextBox;
                    CurrentChatTextBox += $"{tempMessageText}\n";
                    CurrentMessageTextBox = "";
                    _clientObject.SendTextMessage(CurrentChatId, tempMessageText);
                }
            });
        }

        public MainWindowViewModel(IClientObject clientObject) : this()
        {
            _clientObject = clientObject;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    CurrentChatTextBox += _clientObject.GetMessageFromServer() + "\n";
                }
            });
        }

        public MainWindowViewModel() { }
    }
}
