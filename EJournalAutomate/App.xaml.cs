using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Repositories;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Download;
using EJournalAutomate.Services.Logger;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Services.Storage.Cache;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Services.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace EJournalAutomate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger<App>? _logger;
        private string _logPath;

        protected override void OnStartup(StartupEventArgs e)
        {
            _logPath = Path.Combine(Environment.CurrentDirectory, "logs", "app.log");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
                Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddProvider(new LoggingServiceProvider(_logPath));
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddSingleton<ITokenStorage, TokenStorage>()
                .AddSingleton<IAPIService, APIService>()
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

                _logger = Ioc.Default.GetRequiredService<ILogger<App>>();
                _logger.LogInformation($"--- Приложение запущено v{typeof(App).Assembly.GetName().Version} ---");
                _logger.LogInformation($"Платформа: {Environment.OSVersion}, .NET: {Environment.Version}");

                var settingsService = Ioc.Default.GetService<ISettingsStorage>();
                if (settingsService != null)
                {
                    Task.Run(async () =>
                    {
                        await settingsService.LoadSettings();
                        Application.Current.Dispatcher.Invoke(() => {
                            LoggingService.SetSettingsSaveLogs(settingsService.SaveLogs);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                HandleCriticalException(ex);
                Shutdown(-1);
                return;
            }

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleCriticalException(e.Exception);
            e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleCriticalException(e.Exception);
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleCriticalException(e.ExceptionObject as Exception);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger?.LogInformation($"--- Приложение завершено с кодом {e.ApplicationExitCode} ---");
            base.OnExit(e);
        }

        private void HandleCriticalException(Exception exception)
        {
            var crashLogPath = Path.Combine(
                Path.GetDirectoryName(_logPath) ?? Environment.CurrentDirectory,
                $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

            try
            {

                var logger = Ioc.Default.GetService<ILogger<App>>();
                logger?.LogCritical(exception, "Необработанное исключение в приложении");

                LoggingService.SaveLogsOnCrash(crashLogPath);

                MessageBox.Show(
                    $"Произошла критическая ошибка: {exception.Message}\n\nДетали сохранены в лог: {crashLogPath}",
                    "Ошибка приложения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show(
                    $"Критическая ошибка: {exception.Message}",
                    "Ошибка приложения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
