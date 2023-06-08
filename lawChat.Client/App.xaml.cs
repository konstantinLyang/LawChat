using Microsoft.Extensions.Hosting;
using System.Windows;
using lawChat.Client.Services;
using LawChat.Client.Services;
using lawChat.Client.Services.Implementations;
using LawChat.Client.Services.Implementations;
using lawChat.Client.View;
using LawChat.Client.View.Windows;
using lawChat.Client.ViewModel;
using LawChat.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using lawChat.Network.Abstractions;
using LawChat.Network.Implementations;

namespace lawChat.Client
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
