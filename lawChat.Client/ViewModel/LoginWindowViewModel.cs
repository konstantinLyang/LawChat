using System;
using System.Windows.Input;
using lawChat.Client.Infrastructure;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class LoginWindowViewModel : ViewModelBase
    {
        private readonly IClientObject _clientObject;

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

        private LambdaCommand _authorizationCommand;
        public ICommand AuthorizationCommand => _authorizationCommand ??= new(OnAuthorizationCommand, a => true);
        private void OnAuthorizationCommand(object p)
        {
            if (!string.IsNullOrEmpty(LoginTextBox) && !string.IsNullOrEmpty(PasswordTextBox)) _clientObject.OpenConnection(LoginTextBox, PasswordTextBox);
        }

        public LoginWindowViewModel(IClientObject clientObject) : this()
        {
            _clientObject = clientObject;
        }

        public LoginWindowViewModel() { }
    }
}
