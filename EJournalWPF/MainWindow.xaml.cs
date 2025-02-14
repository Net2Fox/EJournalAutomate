using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.DevTools.Network;
using EJournalWPF.Model;
using EJournalWPF.Pages;
using EJournalWPF.Data;

namespace EJournalWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataRepository _dataRepository;
        public MainWindow()
        {
            InitializeComponent();
            _dataRepository = DataRepository.GetInstance();
            if (_dataRepository.IsAuthorized())
            {
                MainFrame.Navigate(new MainPage());
            }
        }

        private void ChangeSaveDirectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DirectorySettingsWindow settingsWindow = new DirectorySettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private async void RedownloadDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await DataRepository.GetInstance().LoadDataFromAPI();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is MainPage)
            {
                RedownloadDataMenuItem.IsEnabled = true;
                SettingsMenuItem.IsEnabled = true;
                DateMailSaveMenuItem.IsChecked = DataRepository.GetInstance().SaveDateTime;
            }
        }

        private void DateMailSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataRepository dataRepository = DataRepository.GetInstance();
            dataRepository.SetDateTimeSave();
            dataRepository.SaveSettings();
            DateMailSaveMenuItem.IsChecked = dataRepository.SaveDateTime;
        }
    }
}
