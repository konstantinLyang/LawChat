using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GongSolutions.Wpf.DragDrop;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;
using lawChat.Server.Data.Model;

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

        private ObservableCollection<SearchPanelModel> _searchPanelSource = new();
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
            if (!string.IsNullOrWhiteSpace(CurrentMessageTextBox) && SelectedChat != null)
            {
                _clientObject.SendPrivateTextMessage(SelectedChat.RecipientId, CurrentMessageTextBox.Trim());
                SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.Messages.Add(new ProcessedMessage()
                {
                    Text = CurrentMessageTextBox.Trim(),
                    CreateDate = DateTime.Now,
                    IsReceivedMessage = false
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
        }

        public void StartListener()
        {
            while (true)
            {
                string message = _clientObject.GetMessageFromServer();

                if (message.Split(';')[0] != "command" && !string.IsNullOrEmpty(message))
                {
                    int senderId = Convert.ToInt32(message.Split(';')[0]);
                    string text = message.Split(';')[1];

                    Dispatcher.Invoke(() =>
                    {
                        SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.Messages.Add(new ProcessedMessage()
                        {
                            Text = text,
                            CreateDate = DateTime.Now,
                            IsReceivedMessage = true
                        });
                    });
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.LastMessage = text;
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == senderId)!.LastMessageDateTime = DateTime.Now;
                }
            }
        }

        public MainWindowViewModel() { }
        public void OnFileDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            _clientObject.SendPrivateFileMessage(SelectedChat.RecipientId, file[0]);
        }
    }
}
