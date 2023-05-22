using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using lawChat.Client.Model;
using lawChat.Client.View;
using lawChat.Client.ViewModel;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using lawChat.Server.Data.Model;
using Microsoft.Extensions.DependencyInjection;
using Message = lawChat.Network.Abstractions.Models.PackageMessage;

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
            if (_mainWindow is { } mainWindow)
            {
                mainWindow.Show();
                return;
            }

            mainWindow = _services.GetRequiredService<MainWindow>();
            _mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            _mainWindow = mainWindow;

            _clientObject.SendMessage(new Message()
            {
                Header = new Header()
                {
                    MessageType = MessageType.Command,
                    StatusCode = StatusCode.GET,
                    CommandArguments = new [] { "friend list" }
                }
            });

            _mainWindowViewModel.UserNameTextBlock =
                _clientData.UserData.LastName + " " +
                _clientData.UserData.FirstName + " " +
                _clientData.UserData.FatherName;

            _loginWindow?.Close();

            _mainWindow.Show();
        }
    }
}
