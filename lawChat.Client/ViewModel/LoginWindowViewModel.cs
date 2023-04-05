using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using lawChat.Client.Infrastructure;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class LoginWindowViewModel : ViewModelBase
    {
        Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly IClientObject _clientObject;
        private readonly IUserDialog _userDialog;

        private string _loginTextBox;
        public string LoginTextBox
        {
            get => _loginTextBox;
            set => Set(ref _loginTextBox, value);
        }

        private string _passwordTextBox;
        public string PasswordTextBox
        {
            get => _passwordTextBox;
            set => Set(ref _passwordTextBox, value);
        }

        private string _infoTextBlock;
        public string InfoTextBlock
        {
            get => _infoTextBlock;
            set => Set(ref _infoTextBlock, value);
        }

        private Brush _borderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,171,173,173)); 
        public Brush BorderBrush
        {
            get => _borderBrush;
            set => Set(ref _borderBrush, value);
        }

        private Brush _textBlocForegroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,255,255,255)); 
        public Brush TextBlocForegroundBrush
        {
            get => _textBlocForegroundBrush;
            set => Set(ref _textBlocForegroundBrush, value);
        }

        private Brush _foregroundInfoTextBlock = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,255,255,255)); 
        public Brush ForegroundInfoTextBlock
        {
            get => _foregroundInfoTextBlock;
            set => Set(ref _foregroundInfoTextBlock, value);
        }

        private Visibility _infoTextBlockVisibility = Visibility.Hidden; 
        public Visibility InfoTextBlockVisibility
        {
            get => _infoTextBlockVisibility;
            set => Set(ref _infoTextBlockVisibility, value);
        }

        private Visibility _loadingIconVisible = Visibility.Hidden; 
        public Visibility LoadingIconVisible
        {
            get => _loadingIconVisible;
            set => Set(ref _loadingIconVisible, value);
        }

        private LambdaCommand _authorizationCommand;
        public ICommand AuthorizationCommand => _authorizationCommand ??= new(OnAuthorizationCommand);
        private void OnAuthorizationCommand(object p)
        {
            Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrEmpty(LoginTextBox) && !string.IsNullOrEmpty(PasswordTextBox))
                {
                    InfoTextBlockVisibility = Visibility.Hidden;
                    LoadingIconVisible = Visibility.Visible;

                    string result = _clientObject.OpenConnection(LoginTextBox, PasswordTextBox);

                    dispatcher.Invoke(() => { BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 171, 173, 173)); });

                    if (result.Contains("successful connection"))
                    {
                        dispatcher.Invoke(() =>
                        {
                            _userDialog.ShowMainWindow();
                        });
                    }
                    else if(result == "incorrect user data")
                    {
                        dispatcher.Invoke(() =>
                        {
                            LoadingIconVisible = Visibility.Hidden;
                            InfoTextBlockVisibility = Visibility.Visible;

                            InfoTextBlock = "Неверный пароль или логин";

                            BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 88, 88));
                            TextBlocForegroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 88, 88));

                            LoginTextBox = "";
                            PasswordTextBox = "";
                        });
                    }
                    else if(result == "user not found")
                    {
                        dispatcher.Invoke(() =>
                        {
                            LoadingIconVisible = Visibility.Hidden;
                            ForegroundInfoTextBlock = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 88, 88));
                            InfoTextBlockVisibility = Visibility.Visible;

                            InfoTextBlock = "Пользователь не найден";
                            LoginTextBox = "";
                            PasswordTextBox = "";
                        });
                    }
                    else
                    {
                        dispatcher.Invoke(() =>
                        {
                            LoadingIconVisible = Visibility.Hidden;
                            ForegroundInfoTextBlock = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 88,88));

                            InfoTextBlockVisibility = Visibility.Visible;

                            InfoTextBlock = "server error";
                        });
                    }
                }
            });
        }

        public LoginWindowViewModel(IClientObject clientObject, IUserDialog userDialog) : this()
        {
            _clientObject = clientObject;
            _userDialog = userDialog;
        }
        public LoginWindowViewModel() { }
    }
}
