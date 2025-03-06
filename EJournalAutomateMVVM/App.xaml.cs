using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
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
            //Ioc.Default.ConfigureServices(
            //    new ServiceCollection()
            //    .AddSingleton<ITokenStorage, TokenStorage>()
            //    .AddSingleton<IApiService, ApiService>()
            //    .AddTransient<ViewModels.LoginViewModel>()
            //    .AddTransient<ViewModels.MainViewModel>()
            //    .BuildServiceProvider());

            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<ITokenStorage, TokenStorage>()
                .AddSingleton<IApiService, ApiService>()
                .AddSingleton<INavigationService, NavigationService>()
                .AddTransient<ViewModels.LoginViewModel>()
                //(provider =>
                //    new ViewModels.LoginViewModel(
                //        provider.GetRequiredService<IApiService>(),
                //        provider.GetRequiredService<INavigationService>()))
                .AddTransient<ViewModels.MainViewModel>()
                //(provider =>
                //    new ViewModels.MainViewModel(
                //        provider.GetRequiredService<IApiService>(),
                //        provider.GetRequiredService<INavigationService>()))
                .BuildServiceProvider());
            base.OnStartup(e);
        }
    }

}
