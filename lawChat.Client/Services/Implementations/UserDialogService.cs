using System;
using System.Windows;
using lawChat.Client.View;
using lawChat.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace lawChat.Client.Services.Implementations
{
    internal class UserDialogService : IUserDialog
    {
        private readonly IServiceProvider _services;
        public UserDialogService(IServiceProvider services)
        {
            _services = services;
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
                mainWindow.Show(); return;
            }

            mainWindow = _services.GetRequiredService<MainWindow>();
            _mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            _mainWindow = mainWindow;

            _loginWindow?.Close();

            _mainWindow.Show();
        }
    }
}
