using Microsoft.Extensions.Hosting;
using System.Windows;
using lawChat.Client.Services;
using lawChat.Client.Services.Implementations;
using lawChat.Client.View;
using lawChat.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;

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
                    services.AddSingleton<IUserDialog, UserDialogService>();
                    services.AddSingleton<IClientObject, ClientObjectService>();

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
