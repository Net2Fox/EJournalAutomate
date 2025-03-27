using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.Download;
using EJournalAutomate.Services.Storage.Cache;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Services.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using EJournalAutomate.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EJournalAutomate.Services.Logger;

namespace EJournalAutomate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var logPath = Path.Combine(Environment.CurrentDirectory, "logs", "app.log");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddProvider(new FileLoggerProvider(logPath));
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddSingleton<ITokenStorage, TokenStorage>()
                .AddSingleton<IApiService, ApiService>()
                .AddSingleton<INavigationService, NavigationService>()
                .AddSingleton<IDispatcherService, DispatcherService>()
                .AddSingleton<ISettingsStorage, SettingsStorage>()
                .AddSingleton<ICacheService, CacheService>()
                .AddSingleton<IMessageRepository, MessageRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IDownloadService, DownloadService>()
                .AddSingleton<ILocalStorage, LocalStorage>()
                .AddTransient<ViewModels.MainWindowViewModel>()
                .AddTransient<ViewModels.LoginViewModel>()
                .AddTransient<ViewModels.MainPageViewModel>()
                .BuildServiceProvider());

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
