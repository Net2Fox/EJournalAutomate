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
using System.Windows.Threading;
using System;

namespace EJournalAutomate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger<App>? _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            var logPath = Path.Combine(Environment.CurrentDirectory, "logs", "app.log");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
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

                _logger = Ioc.Default.GetRequiredService<ILogger<App>>();

                _logger.LogInformation($"--- Приложение запущено v{typeof(App).Assembly.GetName().Version} ---");
                _logger.LogInformation($"Платформа: {Environment.OSVersion}, .NET: {Environment.Version}");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Критическая ошибка при инициализации приложения: {ex}";
                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [CRITICAL] {errorMsg}{Environment.NewLine}");
                MessageBox.Show($"Не удалось запустить приложение: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger?.LogCritical(e.Exception, "Необработанное исключение в UI потоке");
            e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger?.LogCritical(e.Exception, "Необработанное исключение в асинхронной задаче");
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            _logger?.LogCritical(ex, "Необработанное исключение в домене приложения. Фатальное: {Fatal}", e.IsTerminating);
            MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger?.LogInformation($"--- Приложение завершено с кодом {e.ApplicationExitCode} ---");
            base.OnExit(e);
        }
    }
}
