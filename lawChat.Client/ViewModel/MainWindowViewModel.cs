using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Windows.Input;
using System.Windows.Threading;
using LawChat.Client.Assets.CustomNotification;
using lawChat.Client.Infrastructure;
using lawChat.Client.Model;
using LawChat.Client.Model;
using LawChat.Client.Model.Enums;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syroot.Windows.IO;
using ToastNotifications.Core;
using PackageMessage = lawChat.Network.Abstractions.Models.PackageMessage;

namespace lawChat.Client.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        readonly string _downloadsPath = KnownFolders.Downloads.Path;

        private readonly Notifier _notifier;

        readonly SoundPlayer _notification = new ();

        private readonly IClientObject _clientObject;
        private readonly IClientData _clientData;

        private SearchPanelModel _selectedChat;
        public SearchPanelModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                Task.Factory.StartNew(() =>
                {
                    Set(ref _selectedChat, value);
                    OnChatChangedCommand();
                });
            } 
        }

        private Visibility _stickerBlockVisibility = Visibility.Hidden;
        public Visibility StickerBlockVisibility
        {
            get => _stickerBlockVisibility;
            set => Set(ref _stickerBlockVisibility, value);
        }

        public Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        private ObservableCollection<SearchPanelModel> _searchPanelSource = new();
        public ObservableCollection<SearchPanelModel> SearchPanelSource
        {
            get => _searchPanelSource;
            set => Set(ref _searchPanelSource, value);
        }

        private ObservableCollection<StickerModel> _emojiCollection = new();
        public ObservableCollection<StickerModel> EmojiCollection
        {
            get => _emojiCollection;
            set => Set(ref _emojiCollection, value);
        }

        private string? _currentMessageTextBox = "";
        public string? CurrentMessageTextBox
        {
            get => _currentMessageTextBox;
            set => Set(ref _currentMessageTextBox, value);
        }

        private string? _userNameTextBlock;
        public string? UserNameTextBlock
        {
            get => _userNameTextBlock;
            set => Set(ref _userNameTextBlock, value);
        }

        private LambdaCommand _chatChangedCommand;
        public ICommand ChatChangedCommand => _chatChangedCommand ??= new(OnChatChangedCommand);
        private void OnChatChangedCommand()
        {
            if (SelectedChat.IsRead == false) SelectedChat.IsRead = true;
        }

        private void OnOpenFileCommand(object p)
        {
            if (!Directory.Exists(@$"{_downloadsPath}\Downloads"))
                Directory.CreateDirectory(@$"{_downloadsPath}\Downloads");


            var message = SelectedChat.Messages.FirstOrDefault(x => x.Id == (int)p);

            if ((string.IsNullOrEmpty(message.FilePath) || !System.IO.File.Exists(message.FilePath)) && (int)p != 0)
            {
                string CreateFile()
                {
                    try
                    {
                        System.IO.File.Copy(message.ServerFilePath, @$"{_downloadsPath}\Downloads\{message.Text}");

                        return @$"{_downloadsPath}\Downloads\{message.Text}";
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            System.IO.File.Copy(message.ServerFilePath,
                                @$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Text}");

                            return @$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Text}";
                        }

                        catch
                        {
                            return CreateFile();
                        }
                    }
                }

                string createdLocalFilePath = CreateFile();

                message.FilePath = createdLocalFilePath;

                if (IsImage(new FileInfo(createdLocalFilePath)))
                {
                    message.IsImage = true;
                }

                Process.Start("explorer.exe", createdLocalFilePath);

                _clientObject.SendMessage(new()
                {
                    Header = new()
                    {
                        MessageType = MessageType.Command,
                        StatusCode = StatusCode.UPDATE,
                        CommandArguments = new [] { "message recipient filepath", message.FileId.ToString(), createdLocalFilePath }
                    }
                });
            }
            else
            {
                Process.Start("explorer.exe", message.FilePath);
            }
        }
        private void OnOpenFileFolderCommand(object p)
        {
            if (!Directory.Exists(@$"{_downloadsPath}\Downloads"))
                Directory.CreateDirectory(@$"{_downloadsPath}\Downloads");

            var message = SelectedChat.Messages.FirstOrDefault(x => x.Id == (int)p);

            if (string.IsNullOrEmpty(message.FilePath) || !System.IO.File.Exists(message.FilePath))
            {
                string CreateFile()
                {
                    try
                    {
                        System.IO.File.Copy(message.ServerFilePath, @$"{_downloadsPath}\Downloads\{message.Text}");

                        return @$"{_downloadsPath}\Downloads\{message.Text}";
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            System.IO.File.Copy(message.ServerFilePath,
                                @$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Text}");

                            return @$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Text}";
                        }

                        catch
                        {
                            return CreateFile();
                        }
                    }
                }

                string createdLocalFilePath = CreateFile();

                message.FilePath = CreateFile();

                FileInfo fileInfo = new FileInfo(createdLocalFilePath);

                Process.Start("explorer.exe", fileInfo.DirectoryName);

                _clientObject.SendMessage(new()
                {
                    Header = new()
                    {
                        MessageType = MessageType.Command,
                        StatusCode = StatusCode.UPDATE,
                        CommandArguments = new [] { "message recipient filepath", message.FileId.ToString(), fileInfo.FullName }
                    }
                });
            }
            else
            {
                var openFile = new FileInfo(message.FilePath);
                Process.Start("explorer.exe", openFile.DirectoryName);
            }
        }

        private LambdaCommand _sendStickerCommand;
        public ICommand SendStickerCommand => _sendStickerCommand ??= new(OnSendStickerCommand);
        private void OnSendStickerCommand()
        {
            if (StickerBlockVisibility == Visibility.Hidden) StickerBlockVisibility = Visibility.Visible;
            else StickerBlockVisibility = Visibility.Hidden;
        }

        private LambdaCommand _sendFileCommand;
        public ICommand SendFileCommand => _sendFileCommand ??= new(OnSendFileCommand);
        private void OnSendFileCommand()
        {
            if (SelectedChat != null)
            {
                var fd = new OpenFileDialog();

                if (fd.ShowDialog() == true)
                {
                    string fileName = fd.FileName;

                    FileInfo fileInfo = new FileInfo(fileName);

                    byte[] sendBuffer = System.IO.File.ReadAllBytes(fd.FileName);

                    SelectedChat!.Messages
                        .Add(new ProcessedMessage()
                        {
                            Text = fileInfo.Name,
                            CreateDate = DateTime.Now,
                            IsReceivedMessage = false,
                            IsFile = true,
                            FilePath = fileName,
                            IsImage = IsImage(fileInfo),
                            OpenFileCommand = new LambdaCommand(OnOpenFileCommand),
                            OpenFileFolderCommand = new LambdaCommand(OnOpenFileCommand),
                        });

                    SelectedChat.LastMessage = fileInfo.Name;
                    SelectedChat.LastMessageDateTime = DateTime.Now;

                    CurrentMessageTextBox = "";

                    _clientObject.SendMessage(new PackageMessage()
                    {
                        Header = new Header()
                        {
                            MessageType = MessageType.File,
                            CommandArguments = new[]
                                { _selectedChat.RecipientId.ToString(), fileInfo.Name, fileName, } // получатель, имя файла, локальный путь файла.
                        },
                        Data = sendBuffer // файл
                    });
                }
            }
        }

        private LambdaCommand _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand ??= new(OnSendMessageCommand);
        private void OnSendMessageCommand()
        {
            if (!string.IsNullOrWhiteSpace(CurrentMessageTextBox) && SelectedChat != null)
            {
                StickerBlockVisibility = Visibility.Hidden;

                _clientObject.SendMessage(new PackageMessage()
                {
                    Header = new Header()
                    {
                        MessageType = MessageType.Text,
                        CommandArguments = new []{_selectedChat.RecipientId.ToString()}
                    },
                    Data = Encoding.UTF8.GetBytes(CurrentMessageTextBox.Trim())
                });

                Dispatcher.Invoke(() =>
                {
                    SelectedChat!.Messages.Add(new ProcessedMessage()
                    {
                        Text = CurrentMessageTextBox.Trim(),
                        CreateDate = DateTime.Now,
                        IsReceivedMessage = false
                    });

                    SelectedChat.LastMessage = CurrentMessageTextBox;
                    SelectedChat.LastMessageDateTime = DateTime.Now;

                    CurrentMessageTextBox = "";
                });
            }
        }

        private void MessageHandler(object? sender, PackageMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                switch (message.Header.MessageType)
                {
                    case MessageType.Command:
                        switch (message.Header.CommandArguments?[0])
                        {
                            case "friend list":
                                _clientData.FriendList =
                                    JsonConvert.DeserializeObject<List<User>>(Encoding.UTF8.GetString(message.Data));

                                foreach (var friend in _clientData.FriendList)
                                {
                                    if (friend.Id != _clientData.UserData.Id)
                                    {
                                        _clientObject.SendMessage(new PackageMessage()
                                        {
                                            Header = new Header()
                                            {
                                                MessageType = MessageType.Command,
                                                StatusCode = StatusCode.GET,
                                                CommandArguments = new[] { "messages", friend.Id.ToString() }
                                            }
                                        });

                                        SearchPanelSource.Add(new()
                                        {
                                            Title = friend.LastName + " " + friend.FirstName + " " + friend.FatherName,
                                            RecipientId = friend.Id,
                                            LastMessage = "...",
                                            ContactPhoto = friend.PhotoFilePath
                                        });

                                        _clientObject.SendMessage(new PackageMessage()
                                        {
                                            Header = new Header()
                                            {
                                                MessageType = MessageType.Command,
                                                StatusCode = StatusCode.GET,
                                                CommandArguments = new[] { "get connection list" }
                                            }
                                        });
                                    }
                                }
                                break;

                            case "messages":
                                Dispatcher.Invoke(() =>
                                {
                                    var friend = SearchPanelSource.FirstOrDefault(x =>
                                        x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]));

                                    var messages = JsonConvert.DeserializeObject<List<Message>>(Encoding.UTF8.GetString(message.Data));

                                    friend.Messages = new();

                                    foreach (var msg in messages)
                                    {
                                        if (msg.File == null)
                                        {
                                            friend.Messages.Add(new ProcessedMessage()
                                            {
                                                CreateDate = msg.CreateDate,
                                                IsReceivedMessage = msg.SenderId != _clientData.UserData?.Id,
                                                Text = msg.Text,
                                                IsRead = msg.IsRead
                                            });
                                        }
                                        else
                                        {
                                            string filePath;

                                            if (msg.File.Sender.Id == _clientData.UserData?.Id)
                                                filePath = msg.File.SenderLocalFilePath;
                                            else filePath = msg.File.RecipientLocalFilePath;

                                            if (filePath != null)
                                            {
                                                FileInfo fileInfo = new FileInfo(filePath);

                                                friend.Messages.Add(new ProcessedMessage()
                                                {
                                                    Id = msg.Id,
                                                    FileId = msg.File.Id,
                                                    CreateDate = msg.CreateDate,
                                                    IsReceivedMessage = msg.SenderId != _clientData.UserData?.Id,
                                                    Text = msg.File.Name,
                                                    IsFile = true,
                                                    FilePath = filePath,
                                                    IsRead = msg.IsRead,
                                                    IsImage = IsImage(fileInfo),
                                                    ServerFilePath = msg.File.ServerLocalFilePath,
                                                    OpenFileFolderCommand = new LambdaCommand(OnOpenFileFolderCommand),
                                                    OpenFileCommand = new LambdaCommand(OnOpenFileCommand)
                                                });
                                            }
                                            else
                                            {
                                                friend.Messages.Add(new ProcessedMessage()
                                                {
                                                    Id = msg.Id,
                                                    FileId = msg.File.Id,
                                                    CreateDate = msg.CreateDate,
                                                    IsReceivedMessage = msg.SenderId != _clientData.UserData?.Id,
                                                    Text = msg.File.Name,
                                                    IsFile = true,
                                                    IsRead = msg.IsRead,
                                                    ServerFilePath = msg.File.ServerLocalFilePath,
                                                    OpenFileFolderCommand = new LambdaCommand(OnOpenFileFolderCommand),
                                                    OpenFileCommand = new LambdaCommand(OnOpenFileCommand)
                                                });
                                            }
                                        }
                                    }

                                    friend.LastMessage = friend.Messages.Last().Text;
                                    friend.LastMessageDateTime = friend.Messages.Last().CreateDate;
                                    friend.IsRead = friend.Messages.Last().IsRead;
                                });
                                break;

                            case "new client connection":
                                SearchPanelSource.FirstOrDefault(x =>
                                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]))!
                                        .IsOnline =
                                    true;
                                break;

                            case "client close connection":
                                Dispatcher.Invoke(() =>
                                {
                                    SearchPanelSource.FirstOrDefault(x =>
                                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]))!
                                        .IsOnline = false;
                                });
                                break;
                        }
                        break;

                    case MessageType.Text:
                        Dispatcher.Invoke(() =>
                        {
                            SearchPanelSource
                                .FirstOrDefault(x =>
                                    x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]))!
                                .Messages.Add(new ProcessedMessage()
                                {
                                    CreateDate = DateTime.Now,
                                    IsReceivedMessage = true,
                                    Text = Encoding.UTF8.GetString(message.Data)
                                });

                            SearchPanelSource.FirstOrDefault(x => x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]))!.LastMessage = Encoding.UTF8.GetString(message.Data);
                            SearchPanelSource.FirstOrDefault(x => x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]))!.LastMessageDateTime = DateTime.Now;

                            if (SelectedChat !=
                                SearchPanelSource.FirstOrDefault(x => x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0])))
                            {
                                SearchPanelSource.FirstOrDefault(x => x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0])).IsRead = false;
                                try { _notification.Play(); } catch { }
                                _notifier.ShowClientMessage(Encoding.UTF8.GetString(message.Data), SearchPanelSource.FirstOrDefault(x => x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0])).Title, new MessageOptions());
                            }
                        });
                        break;

                    case MessageType.File: // отправитель, имя файла, айди файла
                        if (!Directory.Exists(@$"{_downloadsPath}\Downloads"))
                            Directory.CreateDirectory(@$"{_downloadsPath}\Downloads");

                        string CreateFile()
                        {
                            try
                            {
                                System.IO.File.WriteAllBytes(@$"{_downloadsPath}\Downloads\{message.Header.CommandArguments[1]}", message.Data);

                                return @$"{_downloadsPath}\Downloads\{message.Header.CommandArguments[1]}"; }

                            catch
                            {
                                try
                                {
                                    System.IO.File.WriteAllBytes(@$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}", message.Data);

                                    return @$"{_downloadsPath}\Downloads\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}"; }

                                catch
                                {
                                    return CreateFile();
                                }
                            }
                        }

                        var fileInfo = new FileInfo(CreateFile());

                        var recipient = SearchPanelSource.FirstOrDefault(x =>
                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]));
                        
                        Dispatcher.Invoke(() =>
                        {
                            recipient.Messages.Add(new ProcessedMessage()
                                {
                                    CreateDate = DateTime.Now,
                                    IsReceivedMessage = true,
                                    IsFile = true,
                                    Text = fileInfo.Name,
                                    FilePath = fileInfo.FullName,
                                    IsImage = IsImage(fileInfo),
                            });

                            recipient.LastMessage = fileInfo.Name;
                            recipient.LastMessageDateTime = DateTime.Now;

                            if (SelectedChat != recipient)
                            {
                                recipient.IsRead = false;
                                try { _notification.Play(); } catch { }
                                _notifier.ShowClientMessage($"Файл: { fileInfo.Name }", recipient.Title, new MessageOptions());
                            }
                        });

                        _clientObject.SendMessage(new()
                        {
                            Header = new()
                            {
                                MessageType = MessageType.Command,
                                StatusCode = StatusCode.UPDATE,
                                CommandArguments = new []{ "message recipient filepath", message.Header.CommandArguments[2], fileInfo.FullName }
                            }
                        });

                        break;
                }
            });
        }

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData) : this()
        {
            _clientObject = clientObject;

            _clientData = clientData;

            _clientObject.MessageReceived += MessageHandler;

            _notification.SoundLocation =
                @"Client\data\Sound\notificationSound.wav";

            try{ _notification.Load(); } catch {}

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.Width = 300;
            });

            try
            {
                var files = new DirectoryInfo(@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\Stickers\Emoji").GetFiles();

                foreach (var emoji in files)
                {
                    if (emoji.Extension == ".png")
                    {
                        EmojiCollection.Add(new()
                        {
                            ImageFilePath = emoji.FullName,
                            Name = emoji.Name,
                            Type = EmojiType.Emoji
                        });
                    }
                }
            }
            catch{}

        }

        public MainWindowViewModel() { }

        bool IsImage(FileInfo filePath)
        {
            if ((filePath.Extension == ".jpg" || filePath.Extension == ".png" ||
                 filePath.Extension == ".jpeg" || filePath.Extension == ".bmp" ||
                 filePath.Extension == ".JPG" || filePath.Extension == ".PNG" ||
                 filePath.Extension == ".JPEG" || filePath.Extension == ".BMP") 
                && filePath != null) return true;

            return false;
        }
    }
}
