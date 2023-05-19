using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel;
using lawChat.Client.ViewModel.Base;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using lawChat.Server.Data.Model;
using Newtonsoft.Json;
using PackageMessage = lawChat.Network.Abstractions.Models.PackageMessage;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly IClientObject _clientObject;
        private readonly IClientData _clientData;

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

        private string? _currentMessageTextBox;
        public string? CurrentMessageTextBox
        {
            get => _currentMessageTextBox;
            set => Set(ref _currentMessageTextBox, value);
        }

        private string? _userNameTextBlock;
        public string? UserNameTextBlock
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
                _clientObject.SendMessage(new PackageMessage()
                {
                    Header = new Header()
                    {
                        MessageType = MessageType.Text,
                        CommandArguments = new []{_selectedChat.RecipientId.ToString()}
                    },
                    Data = Encoding.UTF8.GetBytes(CurrentMessageTextBox)
                });

                Dispatcher.Invoke(() =>
                {
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.Messages.Add(new ProcessedMessage()
                    {
                        Text = CurrentMessageTextBox.Trim(),
                        CreateDate = DateTime.Now,
                        IsReceivedMessage = false
                    });

                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.LastMessage = CurrentMessageTextBox;
                    SearchPanelSource.FirstOrDefault(x => x.RecipientId == SelectedChat.RecipientId)!.LastMessageDateTime = DateTime.Now;
                });

                CurrentMessageTextBox = "";
            }
        }

        private void MessageHandler(object? sender, PackageMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                switch (message.Header.MessageType)
                {
                    case MessageType.Command:
                        switch (message.Header.CommandArguments?[0])
                        {
                            case "friend list":
                                _clientData.FriendList =
                                    JsonConvert.DeserializeObject<List<User>>(Encoding.UTF8.GetString(message.Data));

                                Dispatcher.Invoke(() =>
                                {
                                    foreach (var friend in _clientData.FriendList)
                                    {
                                        if (friend.Id != _clientData.UserData.Id)
                                        {
                                            _clientObject.SendMessage(new PackageMessage()
                                            {
                                                Header = new Header()
                                                {
                                                    MessageType = MessageType.Command,
                                                    StatusCode = StatusCode.GET,
                                                    CommandArguments = new[] { "messages", friend.Id.ToString() }
                                                }
                                            });
                                            SearchPanelSource.Add(new()
                                            {
                                                Title = friend.NickName,
                                                RecipientId = friend.Id,
                                                LastMessage = "..."
                                            });
                                        }
                                    }
                                });
                                break;

                            case "messages":
                                Dispatcher.Invoke(() =>
                                {
                                    var friend = SearchPanelSource.FirstOrDefault(x =>
                                        x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]));

                                    var messages = JsonConvert.DeserializeObject<List<Message>>(Encoding.UTF8.GetString(message.Data));

                                    friend.Messages = new();

                                    foreach (var msg in messages)
                                    {
                                        friend.Messages.Add(new ProcessedMessage()
                                        {
                                            CreateDate = msg.CreateDate,
                                            IsReceivedMessage = msg.SenderId != _clientData.UserData?.Id,
                                            Text = msg.Text
                                        });
                                    }

                                    friend.LastMessage = messages.Last().Text;
                                    friend.LastMessageDateTime = messages.Last().CreateDate;
                                });
                                break;
                        }
                        break;
                    case MessageType.Text:
                        Dispatcher.Invoke(() =>
                        {
                            SearchPanelSource
                                .FirstOrDefault(x =>
                                    x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]))
                                .Messages.Add(new ProcessedMessage()
                                {
                                    CreateDate = DateTime.Now,
                                    IsReceivedMessage = true,
                                    Text = Encoding.UTF8.GetString(message.Data)
                                });
                        });
                        break;
                }
            });
        }

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData) : this()
        {
            _clientObject = clientObject;

            _clientData = clientData;

            _clientObject.MessageReceived += MessageHandler;
        }

        public MainWindowViewModel() { }
    }
}
