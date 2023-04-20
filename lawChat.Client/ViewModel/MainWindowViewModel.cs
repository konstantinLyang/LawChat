using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using DataBase.Data.Model;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private IClientObject _clientObject;

        private List<Chat> _chats;
        public List<Chat> Chats
        {
            get => _chats;
            set => Set(ref _chats, value);
        }

        private SearchPanelModel _selectedChat;
        public SearchPanelModel SelectedChat 
        {
            get => _selectedChat;
            set => Set(ref _selectedChat, value);
        }

        public Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private ObservableCollection<SearchPanelModel> _searchPanelSource;
        public ObservableCollection<SearchPanelModel> SearchPanelSource
        {
            get => _searchPanelSource;
            set => Set(ref _searchPanelSource, value);
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
            if (!string.IsNullOrEmpty(CurrentMessageTextBox) && SelectedChat != null)
            {
                _clientObject.SendPrivateTextMessage(SelectedChat.RecipientId, CurrentMessageTextBox);
                SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.Messages.Add(new Message()
                {
                    Text = CurrentMessageTextBox,
                });
                CurrentMessageTextBox = "";
            }
        }

        private LambdaCommand _changeCurrentChat;
        public ICommand ChangeCurrentChat => _changeCurrentChat ??= new(OnChangeCurrentChat);
        private void OnChangeCurrentChat()
        {
        }

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData) : this()
        {
            _clientObject = clientObject;

            SearchPanelSource = new();

            Task.Factory.StartNew(() =>
            {
                StartListener();
            });
        }

        private async void StartListener()
        {
            while (true)
            {
                string a = "A";
                string message = _clientObject.GetMessageFromServer();

                int senderId = Convert.ToInt32(message.Split(';')[0]);
                string text = message.Split(';')[1];

                _dispatcher.Invoke(() =>
                {
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.Messages.Add(new Message()
                    {
                        Text = text,
                    });
                });
            }
        }

        public MainWindowViewModel() { }
    }
}
