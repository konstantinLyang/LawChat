using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;
using lawChat.Server.Data.Model;
using static System.Net.Mime.MediaTypeNames;

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

        public Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        private ObservableCollection<SearchPanelModel> _searchPanelSource;
        public ObservableCollection<SearchPanelModel> SearchPanelSource
        {
            get => _searchPanelSource;
            set => Set(ref _searchPanelSource, value);
        }

        private string _currentMessageTextBox;
        public string CurrentMessageTextBox
        {
            get => _currentMessageTextBox;
            set => Set(ref _currentMessageTextBox, value);
        }

        private string _userNameTextBlock;
        public string UserNameTextBlock
        {
            get => _userNameTextBlock;
            set => Set(ref _userNameTextBlock, value);
        }

        private LambdaCommand _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand ??= new(OnSendMessageCommand);
        private void OnSendMessageCommand()
        {
            if (!string.IsNullOrEmpty(CurrentMessageTextBox) && SelectedChat != null)
            {
                _clientObject.SendPrivateTextMessage(SelectedChat.RecipientId, CurrentMessageTextBox);
                SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.Messages.Add(new ProcessedMessage()
                {
                    Text = CurrentMessageTextBox,
                    CreateDate = DateTime.Now,
                });

                Dispatcher.Invoke(() =>
                {
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.LastMessage = CurrentMessageTextBox;
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.LastMessageDateTime = DateTime.Now;
                });
                
                CurrentMessageTextBox = "";
            }
        }

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData) : this()
        {
            _clientObject = clientObject;

            SearchPanelSource = new();

            Thread listener = new Thread(StartListener);

            listener.Start();
        }

        private void StartListener()
        {
            while (true)
            {
                string message = _clientObject.GetMessageFromServer();

                int senderId = Convert.ToInt32(message.Split(';')[0]);
                string text = message.Split(';')[1];

                Dispatcher.Invoke(() =>
                {
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.Messages.Add(new ProcessedMessage()
                    {
                        Text = text,
                        CreateDate = DateTime.Now,
                    });
                });
                SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.LastMessage = text;
                SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.LastMessageDateTime = DateTime.Now;
            }
        }

        public MainWindowViewModel() { }
    }
}
