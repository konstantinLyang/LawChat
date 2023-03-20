using System.Windows.Input;
using lawChat.Client.Infrastructure;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string _currentChatName = "Общий чат";
        public string CurrentChatName 
        {
            get => _currentChatName;
            set => Set(ref _currentChatName, value);
        }

        private string _currentChatTextBox;
        public string CurrentChatTextBox
        {
            get => _currentChatTextBox;
            set => Set(ref _currentChatTextBox, value);
        }

        private string _currentChatMessageTextBox;
        public string CurrentChatMessageTextBox
        {
            get => _currentChatMessageTextBox;
            set => Set(ref _currentChatMessageTextBox, value);
        }


        private LambdaCommand _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand ??= new(OnSendMessageCommand);
        private void OnSendMessageCommand()
        {

        }

        public MainWindowViewModel() { }
    }
}
