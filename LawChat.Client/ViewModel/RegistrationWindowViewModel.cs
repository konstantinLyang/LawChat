using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using LawChat.Client.Services;
using LawChat.Network.Abstractions.Enums;
using LawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LawChat.Client.ViewModel
{
    public class RegistrationWindowViewModel : BindableBase
    {
        private readonly Dispatcher _dispatcher;

        private readonly IClientObject _clientObject;
        private readonly IUserDialog _userDialog;

        #region Text blocks

        public string? FirstName { get; set; }
        public Brush FirstNameColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? LastName { get; set; }
        public Brush LastNameColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? Login { get; set; }
        public Brush LoginColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? FirstPassword { get; set; }
        public Brush FirstPasswordColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? SecondPassword { get; set; }
        public Brush SecondPasswordColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? NickName { get; set; }
        public Brush NickNameColor { get; set; } = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));

        public string? FatherName { get; set; }

        public string? Email { get; set; }

        public string? UserPhotoFilePath { get; set; }

        public string? InfoTextBlock { get; set; }

        public Visibility LoadingIconVisible { get; set; } = Visibility.Hidden;

        #endregion
        
        public DelegateCommand RegistrationCommand => new(() =>
        {
            InfoTextBlock = "";
            FieldIsNotEmpty();

            if (FirstPassword == SecondPassword)
            {
                if (FieldIsNotEmpty())
                {
                    LoadingIconVisible = Visibility.Visible;
                    InfoTextBlock = "";

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            byte[] sendData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new User()
                            {
                                NickName = NickName.Trim(),
                                FirstName = FirstName.Trim(),
                                LastName = LastName.Trim(),
                                FatherName = FatherName.Trim(),
                                Login = Login.Trim(),
                                Password = SecondPassword,
                                Email = Email.Trim(),
                                PhotoFilePath = UserPhotoFilePath
                            }));

                            PackageMessage answer = _clientObject.SignUp(sendData);

                            if (answer.Header.StatusCode == StatusCode.OK)
                            {
                                _dispatcher.Invoke(() => { _userDialog.ShowMainWindow(); });
                            }
                            else if (answer.Header.StatusCode == StatusCode.ServerError)
                            {
                                _dispatcher.Invoke(() => { LoadingIconVisible = Visibility.Hidden; });
                                InfoTextBlock = "Server error";
                            }
                            else if (answer.Header.StatusCode == StatusCode.Error)
                            {
                                _dispatcher.Invoke(() => { LoadingIconVisible = Visibility.Hidden; });
                                InfoTextBlock = "Пользователь с таким никнеймом или логином уже существует!";
                            }
                        }
                        catch (Exception ex)
                        {
                            LoadingIconVisible = Visibility.Hidden;
                            InfoTextBlock = "Ошибка";
                        }
                    });
                }
            }
            else InfoTextBlock = "Пароли различаются!";

            bool FieldIsNotEmpty()
            {
                int emptyFieldsCount = 0;

                if (string.IsNullOrEmpty(FirstName))
                {
                    FirstNameColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if (string.IsNullOrEmpty(LastName))
                {
                    LastNameColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if (string.IsNullOrEmpty(Login))
                {
                    LoginColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if (string.IsNullOrEmpty(FirstPassword))
                {
                    FirstPasswordColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if (string.IsNullOrEmpty(SecondPassword))
                {
                    SecondPasswordColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if (string.IsNullOrEmpty(NickName))
                {
                    NickNameColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }

                if (emptyFieldsCount == 0) return true;

                InfoTextBlock = "Заполните необходимый поля!";
                return false;
            }
        });

        public DelegateCommand ChangeUserImageCommand => new(() =>
        {
            if (!string.IsNullOrWhiteSpace(NickName))
            {
                if (!Directory.Exists("D:\\Users\\superuser\\Source\\Repos\\konstantinLyang\\LawChat\\lawChat.Client\\bin\\Debug\\net6.0-windows\\client\\data\\Image\\UserPhotos"))
                    Directory.CreateDirectory("D:\\Users\\superuser\\Source\\Repos\\konstantinLyang\\LawChat\\lawChat.Client\\bin\\Debug\\net6.0-windows\\client\\data\\Image\\UserPhotos");

                OpenFileDialog ofd = new();

                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

                if (ofd.ShowDialog() == true)
                {
                    UserPhotoFilePath = ofd.FileName;

                    FileInfo image = new FileInfo(ofd.FileName);

                    if (!System.IO.File.Exists($@"D:\Users\superuser\Source\Repos\konstantinLyang\LawChat\lawChat.Client\bin\Debug\net6.0-windows\client\data\Image\UserPhotos\{NickName}{image.Extension}"))
                    {
                        System.IO.File.Copy(ofd.FileName, $@"D:\Users\superuser\Source\Repos\konstantinLyang\LawChat\lawChat.Client\bin\Debug\net6.0-windows\client\data\Image\UserPhotos\{NickName}{image.Extension}", true);
                    }
                    else
                    {
                        System.IO.File.Delete($@"D:\Users\superuser\Source\Repos\konstantinLyang\LawChat\lawChat.Client\bin\Debug\net6.0-windows\client\data\Image\UserPhotos\{NickName}.{image.Extension}");
                        System.IO.File.Copy(ofd.FileName, $@"D:\Users\superuser\Source\Repos\konstantinLyang\LawChat\lawChat.Client\bin\Debug\net6.0-windows\client\data\Image\UserPhotos\{NickName}{image.Extension}", true);
                    }

                    UserPhotoFilePath =
                        $@"D:\Users\superuser\Source\Repos\konstantinLyang\LawChat\lawChat.Client\bin\Debug\net6.0-windows\client\data\Image\UserPhotos\{NickName}{image.Extension}";
                }
            }
            else
            {
                NickNameColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
            }
        });

        public RegistrationWindowViewModel(IClientObject clientObject, IUserDialog userDialog)
        {
            _clientObject = clientObject;
            _userDialog = userDialog;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
