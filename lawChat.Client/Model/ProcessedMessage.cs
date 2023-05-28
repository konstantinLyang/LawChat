using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using lawChat.Client.ViewModel.Base;

namespace lawChat.Client.Model
{
    public class ProcessedMessage : ViewModelBase
    {
        public int Id { get; set; }
        public int FileId { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsFile { get; set; }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }
        public string? ServerFilePath { get; set; }

        private bool _isImage;
        public bool IsImage
        {
            get => _isImage;
            set => Set(ref _isImage, value);
        }

        public string? Text { get; set; }

        public bool IsReceivedMessage { get; set; }

        public bool IsRead { get; set; }

        private ICommand _openFileFolderCommand;
        public ICommand OpenFileFolderCommand
        {
            get => _openFileFolderCommand;
            set => _openFileFolderCommand = value;
        }

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get => _openFileCommand;
            set => _openFileCommand = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
