using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using LawChat.Client.Services;
using LawChat.Network.Abstractions.Enums;
using LawChat.Network.Abstractions.Models;

namespace LawChat.Client.ViewModel
{
    public class LoginWindowViewModel : BindableBase
    {
        public LoginWindowViewModel(IClientObject clientObject, IUserDialog userDialog)
        {
            _clientObject = clientObject;

            _userDialog = userDialog;

            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private readonly Dispatcher _dispatcher;

        private readonly IClientObject _clientObject;

        private readonly IUserDialog _userDialog;

        #region Elements

        public string? LoginTextBox { get; set; }

        public string? PasswordTextBox { get; set; }

        public string? InfoTextBlock { get; set; }

        public Brush BorderBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public Brush TextBlocForegroundBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        public Brush ForegroundInfoTextBlock { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        public Visibility InfoTextBlockVisibility { get; set; } = Visibility.Hidden;

        public Visibility LoadingIconVisible { get; set; } = Visibility.Hidden;

        #endregion

        #region Commands

        public DelegateCommand OpenRegisterWindowCommand => new DelegateCommand(() => { _userDialog.ShowRegisterWindow(); });

        public AsyncCommand AuthorizationCommand => new(() =>
        {
            return Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrEmpty(LoginTextBox) && !string.IsNullOrEmpty(PasswordTextBox))
                {
                    InfoTextBlockVisibility = Visibility.Hidden;
                    LoadingIconVisible = Visibility.Visible;

                    PackageMessage result = _clientObject.SignIn(LoginTextBox, PasswordTextBox);

                    _dispatcher.Invoke(() => { BorderBrush = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173)); });

                    if (result.Header.StatusCode != StatusCode.ServerError)
                    {
                        if (result.Header.CommandArguments?[0] == "authorization successfully")
                        {
                            _dispatcher.Invoke(() =>
                            {
                                _userDialog.ShowMainWindow();
                            });
                        }
                        else if (result.Header.CommandArguments?[0] == "authorization incorrect user data")
                        {
                            _dispatcher.Invoke(() =>
                            {
                                LoadingIconVisible = Visibility.Hidden;
                                InfoTextBlockVisibility = Visibility.Visible;

                                InfoTextBlock = "Неверный пароль или логин";

                                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                                TextBlocForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));

                                LoginTextBox = "";
                                PasswordTextBox = "";
                            });
                        }
                    }
                    else
                    {
                        _dispatcher.Invoke(() =>
                        {
                            LoadingIconVisible = Visibility.Hidden;
                            ForegroundInfoTextBlock = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));

                            InfoTextBlockVisibility = Visibility.Visible;

                            InfoTextBlock = "server error";
                        });
                    }
                }
            });
        });

        #endregion
    }
}
