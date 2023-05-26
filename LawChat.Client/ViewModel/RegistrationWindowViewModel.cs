using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using lawChat.Client.Infrastructure;
using lawChat.Client.Services;
using lawChat.Client.ViewModel.Base;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LawChat.Client.ViewModel
{
    public class RegistrationWindowViewModel : ViewModelBase
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private readonly IClientObject _clientObject;
        private readonly IUserDialog _userDialog;

        #region Text blocks

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => Set(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }

        private string _fatherName;
        public string FatherName
        {
            get => _fatherName;
            set => Set(ref _fatherName, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _login;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _firstPassword;
        public string FirstPassword
        {
            get => _firstPassword;
            set => Set(ref _firstPassword, value);
        }

        private string _secondPassword;
        public string SecondPassword
        {
            get => _secondPassword;
            set => Set(ref _secondPassword, value);
        }

        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set => Set(ref _nickName, value);
        }

        private string _userPhotoFilePath;
        public string UserPhotoFilePath
        {
            get => _userPhotoFilePath;
            set => Set(ref _userPhotoFilePath, value);
        }

        #endregion

        private LambdaCommand _registrationCommand;
        public ICommand RegistrationCommand => _registrationCommand ??= new(OnRegistrationCommand);
        private void OnRegistrationCommand()
        {
            Task.Factory.StartNew(() =>
            {
                byte[] sendData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new User()
                {
                    NickName = NickName,
                    FirstName = FirstName,
                    LastName = LastName,
                    FatherName = FatherName,
                    Login = Login,
                    Password = FirstPassword,
                    Email = Email,
                    PhotoFilePath = UserPhotoFilePath
                }));

                PackageMessage answer = _clientObject.SignUp(sendData);

                if (answer.Header.StatusCode == StatusCode.OK)
                {
                    _dispatcher.Invoke(() => { _userDialog.ShowMainWindow(); });
                }
            });
        }
        

        private LambdaCommand _changeUserImageCommand;
        public ICommand ChangeUserImageCommand => _changeUserImageCommand ??= new(OnChangeUserImageCommand);
        private void OnChangeUserImageCommand()
        {
            OpenFileDialog ofd = new();

            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

            if (ofd.ShowDialog() == true)
            {
                UserPhotoFilePath = ofd.FileName;

                FileInfo image = new FileInfo(ofd.FileName);

                if (!System.IO.File.Exists($@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\UserPhotos\{NickName}.{image.Extension}"))
                {
                    System.IO.File.Copy(ofd.FileName, $@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\UserPhotos\{NickName}.{image.Extension}", true);
                }
                else
                {
                    System.IO.File.Delete($@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\UserPhotos\{NickName}.{image.Extension}");
                    System.IO.File.Copy(ofd.FileName, $@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\UserPhotos\{NickName}.{image.Extension}", true);
                }

                UserPhotoFilePath =
                    $@"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\UserPhotos\{NickName}.{image.Extension}";
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
