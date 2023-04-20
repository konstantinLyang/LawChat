using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DataBase.Data;
using DataBase.Data.Model;
using lawChat.Client.View;
using lawChat.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace lawChat.Client.Services.Implementations
{
    internal class UserDialogService : IUserDialog
    {
        private readonly IServiceProvider _services;
        private readonly IClientData _clientData;
        public UserDialogService(IServiceProvider services, IClientData clientData)
        {
            _services = services;
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
            _clientData.GetFriendList();
            _clientData.GetChatList();

            if (_mainWindow is { } mainWindow)
            {
                mainWindow.Show(); return;
            }

            mainWindow = _services.GetRequiredService<MainWindow>();
            _mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            _mainWindow = mainWindow;
            
            _loginWindow?.Close();

            _mainWindowViewModel._dispatcher.Invoke(() =>
            {
                foreach (var friend in _clientData.FriendList)
                {
                    _mainWindowViewModel.SearchPanelSource.Add(new()
                    {
                        Title = friend.NickName,
                        RecipientId = _clientData.FriendList.FirstOrDefault(x => x.NickName == friend.NickName).Id,
                        Messages = GetMessages(_clientData.ClientData.Id, _clientData.FriendList.FirstOrDefault(x => x.NickName == friend.NickName).Id)
                    });
                }
                _mainWindowViewModel.Chats = _clientData.ChatList;
            });

            ObservableCollection<Message> GetMessages(int sender, int recipient)
            {
                ObservableCollection<Message> result = new();

                using (var context = new LawChatDbContext())
                {
                    foreach (var message in context.Messages)
                    {
                        try
                        {
                            if (message.RecipientId == recipient && message.SenderId == sender) result.Add(message);
                            if (message.RecipientId == sender && message.SenderId == recipient) result.Add(message);
                        }
                        catch {}
                    }
                }

                return result;
            }

            _mainWindow.Show();
        }
    }
}
