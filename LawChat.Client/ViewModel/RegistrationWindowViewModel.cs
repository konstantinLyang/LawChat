using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LawChat.Client.Infrastructure;
using LawChat.Client.Services;
using LawChat.Client.ViewModel.Base;
using LawChat.Network.Abstractions.Enums;
using LawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LawChat.Client.ViewModel
{
    public class RegistrationWindowViewModel : ViewModelBase
    {
        private readonly Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        private readonly IClientObject _clientObject;
        private readonly IUserDialog _userDialog;

        #region Text blocks

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set => Set(ref _firstName, value);
        }

        private Brush _firstNameColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush FirstNameColor
        {
            get => _firstNameColor;
            set => Set(ref _firstNameColor, value);
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }

        private Brush _lastNameColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush LastNameColor
        {
            get => _lastNameColor;
            set => Set(ref _lastNameColor, value);
        }

        private string _fatherName = "";
        public string FatherName
        {
            get => _fatherName;
            set => Set(ref _fatherName, value);
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _login = "";
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private Brush _loginColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush LoginColor
        {
            get => _loginColor;
            set => Set(ref _loginColor, value);
        }

        private string _firstPassword = "";
        public string FirstPassword
        {
            get => _firstPassword;
            set => Set(ref _firstPassword, value);
        }

        private Brush _firstPasswordColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush FirstPasswordColor
        {
            get => _firstPasswordColor;
            set => Set(ref _firstPasswordColor, value);
        }

        private string _secondPassword = "";
        public string SecondPassword
        {
            get => _secondPassword;
            set => Set(ref _secondPassword, value);
        }

        private Brush _secondPasswordColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush SecondPasswordColor
        {
            get => _secondPasswordColor;
            set => Set(ref _secondPasswordColor, value);
        }

        private string _nickName = "";
        public string NickName
        {
            get => _nickName;
            set => Set(ref _nickName, value);
        }

        private Brush _nickNameColor = new SolidColorBrush(Color.FromArgb(255, 171, 173, 173));
        public Brush NickNameColor
        {
            get => _nickNameColor;
            set => Set(ref _nickNameColor, value);
        }

        private string _userPhotoFilePath = "";
        public string UserPhotoFilePath
        {
            get => _userPhotoFilePath;
            set => Set(ref _userPhotoFilePath, value);
        }

        private string _infoTextBlock = "";
        public string InfoTextBlock
        {
            get => _infoTextBlock;
            set => Set(ref _infoTextBlock, value);
        }

        private Visibility _loadingIconVisible = Visibility.Hidden;
        public Visibility LoadingIconVisible
        {
            get => _loadingIconVisible;
            set => Set(ref _loadingIconVisible, value);
        }

        #endregion

        private LambdaCommand _registrationCommand;
        public ICommand RegistrationCommand => _registrationCommand ??= new(OnRegistrationCommand);
        private void OnRegistrationCommand()
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
                                Dispatcher.Invoke(() => { _userDialog.ShowMainWindow(); });
                            }
                            else if (answer.Header.StatusCode == StatusCode.ServerError)
                            {
                                Dispatcher.Invoke(() => { LoadingIconVisible = Visibility.Hidden; });
                                InfoTextBlock = "Server error";
                            }
                            else if (answer.Header.StatusCode == StatusCode.Error)
                            {
                                Dispatcher.Invoke(() => { LoadingIconVisible = Visibility.Hidden; });
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
                if(string.IsNullOrEmpty(Login))
                {
                    LoginColor = new SolidColorBrush(Color.FromArgb(255, 255, 88, 88));
                    emptyFieldsCount++;
                }
                if(string.IsNullOrEmpty(FirstPassword))
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
        }
        
        private LambdaCommand _changeUserImageCommand;
        public ICommand ChangeUserImageCommand => _changeUserImageCommand ??= new(OnChangeUserImageCommand);
        private void OnChangeUserImageCommand()
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
        }

        public RegistrationWindowViewModel(IClientObject clientObject, IUserDialog userDialog) : this()
        {
            _clientObject = clientObject;
            _userDialog = userDialog;
        }
        public RegistrationWindowViewModel(){}
    }
}
