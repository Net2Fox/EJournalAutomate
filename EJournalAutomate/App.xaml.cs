using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Services.API;
using EJournalAutomateMVVM.Services.Navigation;
using EJournalAutomateMVVM.Services.Storage;
using EJournalAutomateMVVM.Services.UI;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace EJournalAutomateMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
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
                .AddTransient<ViewModels.LoginViewModel>()
                .AddTransient<ViewModels.MainViewModel>()
                .BuildServiceProvider());

            base.OnStartup(e);
        }
    }

}
