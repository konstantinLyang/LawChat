using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class LoginWindowViewModel : ViewModelBase
    {
        private string? _loginTextBox;
        public string? LoginTextBox
        {
            get => _loginTextBox;
            set => Set(ref _loginTextBox, value);
        }

        private string? _passwordTextBox;
        public string? PasswordTextBox
        {
            get => _passwordTextBox;
            set => Set(ref _passwordTextBox, value);
        }
    }
}
