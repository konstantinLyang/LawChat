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

        private Window LoginWindow;
        private Window MainWindow;
        
        private MainWindowViewModel _mainWindowViewModel;

        public void ShowLoginWindow()
        {
            LoginWindow = _services.GetRequiredService<LoginWindow>();
            LoginWindow.Show();
        }

        public void ShowMainWindow()
        {
            if (MainWindow is { } mainWindow)
            {
                mainWindow.Show(); return;
            }

            mainWindow = _services.GetRequiredService<MainWindow>();
            _mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
        }
    }
}
