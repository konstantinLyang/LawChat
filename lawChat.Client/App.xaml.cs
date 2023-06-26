using Microsoft.Extensions.Hosting;
using System.Windows;
using LawChat.Client.Services;
using LawChat.Client.Services.Implementations;
using LawChat.Client.View;
using LawChat.Client.View.Windows;
using LawChat.Client.ViewModel;
using LawChat.Network.Abstractions;
using LawChat.Network.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace LawChat.Client
{
    public partial class App : Application
    {
        public static IHost? AppHost;
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<LoginWindowViewModel>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<RegistrationWindowViewModel>();
                    services.AddSingleton<IUserDialog, UserDialogService>();
                    services.AddSingleton<IClientObject, ClientObjectService>();
                    services.AddSingleton<IClientData, ClientDataService>();
                    services.AddSingleton<IConnection, Connection>();
                    services.AddTransient<IDataBase, DataBaseService>();

                    services.AddTransient(s =>
                    {
                        var model = s.GetService<LoginWindowViewModel>();
                        var view = new LoginWindow { DataContext = model } ;

                        return view;
                    });
                    services.AddTransient(s =>
                    {
                        var model = s.GetService<MainWindowViewModel>();
                        var view = new MainWindow { DataContext = model }; ;

                        return view;
                    });
                    services.AddTransient(s =>
                    {
                        var model = s.GetService<RegistrationWindowViewModel>();
                        var view = new RegistrationWindow { DataContext = model }; ;

                        return view;
                    });

                }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<IUserDialog>();
            startupForm.ShowLoginWindow();

            base.OnStartup(e);
        }
    }
}
