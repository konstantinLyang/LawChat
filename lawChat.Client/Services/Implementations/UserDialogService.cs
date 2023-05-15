using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using lawChat.Client.Model;
using lawChat.Client.View;
using lawChat.Client.ViewModel;
using lawChat.Server.Data;
using lawChat.Server.Data.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace lawChat.Client.Services.Implementations
{
    internal class UserDialogService : IUserDialog
    {
        private readonly IServiceProvider _services;
        private readonly IClientData _clientData;
        private readonly IClientObject _clientObject;

        public UserDialogService(IServiceProvider services, IClientData clientData, IClientObject clientObject)
        {
            _services = services;
            _clientData = clientData;
            _clientObject = clientObject;
            _clientData = clientData;
        }

        private Window? _loginWindow;
        private Window? _mainWindow;
        
        private MainWindowViewModel? _mainWindowViewModel;

        public void ShowLoginWindow()
        {
            _loginWindow = _services.GetRequiredService<LoginWindow>();
            _loginWindow.Show();
        }

        public void ShowMainWindow()
        {
            _clientObject.SendServerCommandMessage("getdialoglist;");

            _clientData.CommandServerReceivedHandler(_clientObject.GetMessageFromServer());

            _clientObject.SendServerCommandMessage("getuserdata;");

            _clientData.CommandServerReceivedHandler(_clientObject.GetMessageFromServer());

            if (_mainWindow is { } mainWindow)
            {
                mainWindow.Show(); return;
            }

            mainWindow = _services.GetRequiredService<MainWindow>();
            _mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            _mainWindow = mainWindow;

            _mainWindowViewModel.Dispatcher.Invoke(() =>
            {
                foreach (var friend in _clientData.FriendList)
                {
                    var messages = GetMessages(_clientData.UserData.Id, friend.Id);

                    if (friend.Id == _clientData.UserData.Id)
                    {
                        if (messages.Count != 0)
                        {
                            _mainWindowViewModel.SearchPanelSource.Add(new()
                            {
                                Title = "Избранное",
                                RecipientId = friend.Id,
                                Messages = messages,
                                LastMessage = messages.Last().Text,
                                LastMessageDateTime = messages.Last().CreateDate

                            });
                        }
                        else
                        {
                            _mainWindowViewModel.SearchPanelSource.Add(new()
                            {
                                Title = "Избранное",
                                RecipientId = friend.Id,
                                Messages = messages,
                                LastMessage = "...",
                                LastMessageDateTime = null

                            });
                        }
                    }
                    else
                    {
                        if (messages.Count != 0)
                        {
                            _mainWindowViewModel.SearchPanelSource.Add(new()
                            {
                                Title = friend.NickName,
                                RecipientId = friend.Id,
                                Messages = messages,
                                LastMessage = messages.Last().Text,
                                LastMessageDateTime = messages.Last().CreateDate
                            });
                        }
                        else
                        {
                            _mainWindowViewModel.SearchPanelSource.Add(new()
                            {
                                Title = friend.NickName,
                                RecipientId = friend.Id,
                                Messages = messages,
                                LastMessage = "...",
                                LastMessageDateTime = null
                            });
                        }
                    }
                }
                _mainWindowViewModel.Chats = _clientData.ChatList;
            });

            _mainWindowViewModel.UserNameTextBlock = _clientData.UserData?.NickName;

            ObservableCollection<ProcessedMessage> GetMessages(int sender, int recipient)
            {
                ObservableCollection<ProcessedMessage> result = new();

                _clientObject.SendServerCommandMessage($"getmessages;{sender};{recipient}");

                string a = _clientObject.GetMessageFromServer();

                var messages = JsonConvert.DeserializeObject<List<Message>>(a.Split(';')[2]);

                if (messages != null)
                    foreach (var message in messages)
                    {
                        try
                        {
                            if (message.RecipientId == recipient && message.SenderId == sender)
                                result.Add(new ProcessedMessage()
                                {
                                    CreateDate = message.CreateDate,
                                    Id = message.Id,
                                    Text = message.Text,
                                    IsReceivedMessage = IsReceivedMessage(message.SenderId)
                                });
                            if (message.RecipientId == sender && message.SenderId == recipient)
                                result.Add(new ProcessedMessage()
                                {
                                    CreateDate = message.CreateDate,
                                    Id = message.Id,
                                    Text = message.Text,
                                    IsReceivedMessage = IsReceivedMessage(message.SenderId)
                                });
                        }
                        catch
                        {
                            throw new Exception();
                        }
                    }

                return result;
            }

            bool IsReceivedMessage(int sender)
            {
                if(_clientData.UserData.Id == sender) return false;
                return true;
            }

            _loginWindow?.Close();

            Thread listener = new Thread(_mainWindowViewModel.StartListener);

            listener.Start();

            _mainWindow.Show();
        }
    }
}
