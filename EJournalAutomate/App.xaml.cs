using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Repositories;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Cache;
using EJournalAutomate.Services.Download;
using EJournalAutomate.Services.Logger;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Services.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using EJournalAutomate.Views;
using Microsoft.Extensions.Hosting;

namespace EJournalAutomate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        
        private ILogger<App>? _logger;
        private string _logPath;

        protected async override void OnStartup(StartupEventArgs e)
        {
            var builder = Host.CreateApplicationBuilder();
            
            _logPath = Path.Combine(Environment.CurrentDirectory, "logs", "app.log");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
               builder.Services.AddLogging(builder =>
               {
                   builder.AddProvider(new LoggingServiceProvider(_logPath));
                   builder.SetMinimumLevel(LogLevel.Information);
               });
               builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
               builder.Services.AddSingleton<ITokenStorage, TokenStorage>();
               builder.Services.AddSingleton<IAPIService, APIService>();
               builder.Services.AddSingleton<INavigationService, NavigationService>();
               builder.Services.AddSingleton<IDispatcherService, DispatcherService>();
               builder.Services.AddSingleton<ISettingsStorage, SettingsStorage>();
               builder.Services.AddSingleton<ICacheService, CacheService>();
               builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
               builder.Services.AddSingleton<IUserRepository, UserRepository>();
               builder.Services.AddSingleton<IDownloadService, DownloadService>();
               builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
               // ViewModels
               builder.Services.AddTransient<ViewModels.LoginViewModel>();
               builder.Services.AddTransient<ViewModels.MainPageViewModel>();
               // Pages
               builder.Services.AddTransient<Views.Pages.LoginPage>();
               builder.Services.AddTransient<Views.Pages.MainPage>();
               // MainWindow и MainWindowViewModel
               builder.Services.AddSingleton<ViewModels.MainWindowViewModel>();
               builder.Services.AddSingleton<MainWindow>();

                _host = builder.Build();
                
                await _host.StartAsync();
                
                _logger = _host.Services.GetRequiredService<ILogger<App>>();
                _logger.LogInformation($"--- Приложение запущено v{typeof(App).Assembly.GetName().Version} ---");
                _logger.LogInformation($"Платформа: {Environment.OSVersion}, .NET: {Environment.Version}");

                var settingsService = _host.Services.GetService<ISettingsStorage>();
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
                
                MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
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

                var logger = _host.Services.GetService<ILogger<App>>();
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
