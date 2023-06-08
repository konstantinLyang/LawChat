using System;
using System.Windows;
using lawChat.Client.View;
using LawChat.Client.View.Windows;
using lawChat.Client.ViewModel;
using LawChat.Client.ViewModel;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

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

        private Window? _registrationWindow;


        private MainWindowViewModel? _mainWindowViewModel;

        private RegistrationWindowViewModel? _registrationWindowViewModel;

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

            _clientObject.SendMessage(new PackageMessage()
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

            _mainWindowViewModel.UserPhoto = _clientData.UserData.PhotoFilePath;

            _loginWindow?.Close();
            _registrationWindow?.Close();

            _mainWindow.Show();
        }

        public void ShowRegisterWindow()
        {
            if (_registrationWindow is { } registrationWindow)
            {
                try{ registrationWindow.Show(); } catch{ /* ignored */ }
                return;
            }

            _registrationWindow = _services.GetRequiredService<RegistrationWindow>();
            _registrationWindow.Show();
        }
    }
}
