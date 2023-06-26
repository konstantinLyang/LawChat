﻿using System;
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
using DevExpress.Mvvm;
using LawChat.Client.Assets.CustomNotification;
using LawChat.Client.Infrastructure;
using LawChat.Client.Model;
using LawChat.Client.Model.Enums;
using LawChat.Client.Services;
using LawChat.Client.ViewModel.Base;
using LawChat.Network.Abstractions.Enums;
using LawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syroot.Windows.IO;
using ToastNotifications.Core;
using PackageMessage = LawChat.Network.Abstractions.Models.PackageMessage;

namespace LawChat.Client.ViewModel
{
    internal class MainWindowViewModel : BindableBase
    {
        readonly string _downloadsPath = KnownFolders.Downloads.Path;

        public Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        private readonly Notifier _notifier;

        readonly SoundPlayer _notification = new ();

        private readonly IClientObject _clientObject;
        private readonly IClientData _clientData;
        
        public Visibility StickerBlockVisibility { get; set; } = Visibility.Hidden;
        public Visibility RightPanelVisibility { get; set; } = Visibility.Hidden;
        public SearchPanelModel SelectedChat { get; set; } = new ();
        public ObservableCollection<SearchPanelModel> AllDialogCollection { get; set; } = new();
        public ObservableCollection<SearchPanelModel> MainDialogCollection { get; set; } = new();
        public ObservableCollection<StickerModel> EmojiCollection { get; set; } = new();
        public string? CurrentMessageTextBox { get; set; }
        public string? SearchBox { get; set; }
        public string? UserNameTextBlock { get; set; }
        public string? UserPhoto { get; set; }

        #region Commands

        public DelegateCommand OpenRightPanelCommand => new(() =>
        {
            if (RightPanelVisibility == Visibility.Visible) RightPanelVisibility = Visibility.Hidden;
            else RightPanelVisibility = Visibility.Visible;
        });
        public DelegateCommand ChatChangedCommand => new(() =>
        {
            try
            {
                if (SelectedChat.Messages.Count > 0)
                {
                    var nonReadMessages = SelectedChat.Messages.Where(x => x.IsRead == false && x.IsReceivedMessage == true);
                    foreach (var nonReadMessage in nonReadMessages)
                    {
                        _clientObject.SendMessage(new PackageMessage()
                        {
                            Header = new Header()
                            {
                                MessageType = MessageType.Command,
                                StatusCode = StatusCode.UPDATE,
                                CommandArguments = new[] { "isread", nonReadMessage.Id.ToString() }
                            }
                        });
                    }
                    if (SelectedChat.IsRead == false) SelectedChat.IsRead = true;
                }
            }
            catch { /* ignored */ }
        });
        public DelegateCommand SendStickerCommand => new(() =>
        {
            if (StickerBlockVisibility == Visibility.Hidden) StickerBlockVisibility = Visibility.Visible;
            else StickerBlockVisibility = Visibility.Hidden;
        });
        public DelegateCommand SendFileCommand => new(() =>
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
                            Id = new Random().Next(1, 1247512577),
                            Text = fileInfo.Name,
                            CreateDate = DateTime.Now,
                            IsReceivedMessage = false,
                            IsFile = true,
                            FilePath = fileName,
                            IsImage = IsImage(fileInfo),
                            OpenFileCommand = new LambdaCommand(OnOpenFileCommand),
                            OpenFileFolderCommand = new LambdaCommand(OnOpenFileFolderCommand),
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
                                { SelectedChat.RecipientId.ToString(), fileInfo.Name, fileName, } // получатель, имя файла, локальный путь файла.
                        },
                        Data = sendBuffer // файл
                    });
                }
            }
        });
        public DelegateCommand SendMessageCommand => new(() =>
        {
            if (!string.IsNullOrWhiteSpace(CurrentMessageTextBox) && SelectedChat != null)
            {
                StickerBlockVisibility = Visibility.Hidden;

                _clientObject.SendMessage(new PackageMessage()
                {
                    Header = new Header()
                    {
                        MessageType = MessageType.Text,
                        CommandArguments = new[] { SelectedChat.RecipientId.ToString() }
                    },
                    Data = Encoding.UTF8.GetBytes(CurrentMessageTextBox.Trim())
                });

                Dispatcher.Invoke(() =>
                {
                    SelectedChat!.Messages.Add(new ProcessedMessage()
                    {
                        Text = CurrentMessageTextBox.Trim(),
                        CreateDate = DateTime.Now,
                        IsReceivedMessage = false,
                    });

                    SelectedChat.LastMessage = CurrentMessageTextBox;
                    SelectedChat.LastMessageDateTime = DateTime.Now;

                    CurrentMessageTextBox = "";
                });
            }
        });
        public AsyncCommand FindUserCommand => new(() =>
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(SearchBox))
                    {
                        var a = AllDialogCollection.Where(x => x.Title.Contains(SearchBox));

                        MainDialogCollection = new();

                        foreach (var item in a)
                        {
                            MainDialogCollection.Add(item);
                        }
                    }
                    else
                    {
                        MainDialogCollection = AllDialogCollection;
                    }
                }
                catch (Exception ex)
                {
                    MainDialogCollection = AllDialogCollection;
                }
            });
        });

        #endregion

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
                        catch { return CreateFile(); }
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
                        CommandArguments = new[] { "message recipient filepath", message.FileId.ToString(), createdLocalFilePath }
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
                        CommandArguments = new[] { "message recipient filepath", message.FileId.ToString(), fileInfo.FullName }
                    }
                });
            }
            else
            {
                var openFile = new FileInfo(message.FilePath);
                Process.Start("explorer.exe", openFile.DirectoryName);
            }
        }
        private void GetEmoji()
        {
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
            catch { }
        }
        private static bool IsImage(FileInfo filePath)
        {
            if ((filePath.Extension == ".jpg" || filePath.Extension == ".png" ||
                 filePath.Extension == ".jpeg" || filePath.Extension == ".bmp" ||
                 filePath.Extension == ".JPG" || filePath.Extension == ".PNG" ||
                 filePath.Extension == ".JPEG" || filePath.Extension == ".BMP")
                && filePath != null) return true;

            return false;
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

                                        AllDialogCollection.Add(new()
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
                                    var friend = AllDialogCollection.FirstOrDefault(x =>
                                        x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]));

                                    var messages = JsonConvert.DeserializeObject<List<Message>>(Encoding.UTF8.GetString(message.Data));

                                    friend.Messages = new();

                                    foreach (var msg in messages)
                                    {
                                        if (msg.File == null)
                                        {
                                            friend.Messages.Add(new ProcessedMessage()
                                            {
                                                Id = msg.Id,
                                                CreateDate = msg.CreateDate,
                                                IsReceivedMessage = msg.SenderId != _clientData.UserData?.Id,
                                                Text = msg.Text,
                                                IsRead = IsRead()
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
                                                    IsRead = IsRead(),
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
                                                    IsRead = IsRead(),
                                                    ServerFilePath = msg.File.ServerLocalFilePath,
                                                    OpenFileFolderCommand = new LambdaCommand(OnOpenFileFolderCommand),
                                                    OpenFileCommand = new LambdaCommand(OnOpenFileCommand)
                                                });
                                            }
                                        }
                                        bool IsRead()
                                        {
                                            if (msg.SenderId != _clientData.UserData?.Id)
                                            {
                                                if (!msg.IsRead)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                            return true;
                                        }
                                    }

                                    friend.LastMessage = friend.Messages.Last().Text;
                                    friend.LastMessageDateTime = friend.Messages.Last().CreateDate;
                                    friend.IsRead = friend.Messages.Last().IsRead;
                                });
                                break;

                            case "new client connection":
                                AllDialogCollection.FirstOrDefault(x =>
                                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]))!
                                        .IsOnline =
                                    true;
                                break;

                            case "client close connection":
                                Dispatcher.Invoke(() =>
                                {
                                    AllDialogCollection.FirstOrDefault(x =>
                                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[1]))!
                                        .IsOnline = false;
                                });
                                break;
                        }
                        break;

                    case MessageType.Text:
                        Dispatcher.Invoke(() =>
                        {
                            var chat = AllDialogCollection
                                .FirstOrDefault(x =>
                                    x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]));

                            if (chat != null)
                            {
                                chat.Messages.Add(new ProcessedMessage()
                                {
                                    Id = Convert.ToInt32(message.Header.CommandArguments[1]),
                                    CreateDate = DateTime.Now,
                                    IsReceivedMessage = true,
                                    Text = Encoding.UTF8.GetString(message.Data)
                                });

                                chat.LastMessage = Encoding.UTF8.GetString(message.Data);
                                chat.LastMessageDateTime = DateTime.Now;

                                if (SelectedChat != chat)
                                {
                                    chat.IsRead = false;
                                    try { _notification.Play(); } catch { }
                                    _notifier.ShowClientMessage(Encoding.UTF8.GetString(message.Data), chat.Title, chat.ContactPhoto, new MessageOptions());
                                }
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

                        var recipient = AllDialogCollection.FirstOrDefault(x =>
                            x.RecipientId == Convert.ToInt32(message.Header.CommandArguments[0]));
                        
                        Dispatcher.Invoke(() =>
                        {
                            recipient.Messages.Add(new ProcessedMessage()
                                {
                                    Id = Convert.ToInt32(message.Header.CommandArguments[3]),
                                    CreateDate = DateTime.Now,
                                    IsReceivedMessage = true,
                                    IsFile = true,
                                    Text = fileInfo.Name,
                                    FilePath = fileInfo.FullName,
                                    FileId = Convert.ToInt32(message.Header.CommandArguments[2]),
                                    IsImage = IsImage(fileInfo),
                                    OpenFileCommand = new LambdaCommand(OnOpenFileCommand),
                                    OpenFileFolderCommand = new LambdaCommand(OnOpenFileFolderCommand)
                                });

                            recipient.LastMessage = fileInfo.Name;
                            recipient.LastMessageDateTime = DateTime.Now;

                            if (SelectedChat != recipient)
                            {
                                recipient.IsRead = false;
                                try { _notification.Play(); } catch { }
                                _notifier.ShowClientMessage($"Файл: { fileInfo.Name }", recipient.Title, recipient.ContactPhoto, new MessageOptions());
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

        public MainWindowViewModel(IClientObject clientObject, IClientData clientData)
        {
            MainDialogCollection = AllDialogCollection;

            _clientObject = clientObject;

            _clientData = clientData;

            _clientObject.MessageReceived += MessageHandler;

            _notification.SoundLocation = @"Client\data\Sound\notificationSound.wav";

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

                cfg.DisplayOptions.Width = 400;
            });

            GetEmoji();
        }
    }
}
