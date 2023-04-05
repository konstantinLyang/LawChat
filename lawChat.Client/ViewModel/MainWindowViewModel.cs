using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private IClientObject _clientObject;
        private IClientData _clientData;

        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private ObservableCollection<SearchPanelModel> _searchPanelSource;

        public ObservableCollection<SearchPanelModel> SearchPanelSource
        {
            get => _searchPanelSource;
            set => Set(ref _searchPanelSource, value);
        }

        private string _currentChatName = "Общий чат";
        public string CurrentChatName 
        {
            get => _currentChatName;
            set => Set(ref _currentChatName, value);
        }

        private int _currentChatId = 0;
        public int CurrentChatId
        {
            get => _currentChatId;
            set => Set(ref _currentChatId, value);
        }

        private string _currentChatTextBox;
        public string CurrentChatTextBox
        {
            get => _currentChatTextBox;
            set => Set(ref _currentChatTextBox, value);
        }

        private string _currentMessageTextBox;
        public string CurrentMessageTextBox
        {
            get => _currentMessageTextBox;
            set => Set(ref _currentMessageTextBox, value);
        }

        private LambdaCommand _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand ??= new(OnSendMessageCommand);
        private void OnSendMessageCommand()
        {
            Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrEmpty(CurrentMessageTextBox))
                {
                    var tempMessageText = CurrentMessageTextBox;
                    CurrentChatTextBox += $"{tempMessageText}\n";
                    CurrentMessageTextBox = "";
                    _clientObject.SendTextMessage(CurrentChatId, tempMessageText);
                }
            });
        }

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData) : this()
        {
            _clientObject = clientObject;
            _clientData = clientData;

            SearchPanelSource = new();

            Task.Factory.StartNew(() =>
            {
                _clientObject.ClientSocket.Send(Encoding.Unicode.GetBytes("speccommand|getfriendlist"));
                while (true)
                {
                    if (_clientData.FriendList.Count != 0)
                    {
                        foreach (var client in _clientData.FriendList)
                        {
                            _dispatcher.Invoke(() =>
                            {
                                SearchPanelSource.Add(new()
                                {
                                    Title = client.NickName
                                });
                            });
                        }
                        return;
                    }
                    Thread.Sleep(1000);
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    string message = _clientObject.GetMessageFromServer();
                    if (message != "gaose12h3ksafhai82t")
                    {
                        CurrentChatTextBox += message + "\n";
                    }
                }
            });
        }

        public MainWindowViewModel() { }
    }
}
