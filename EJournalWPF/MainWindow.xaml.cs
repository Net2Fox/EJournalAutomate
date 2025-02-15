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
using EJournalWPF.Model;
using EJournalWPF.Pages;
using EJournalWPF.Data;
using EJournalAutomate.Data;

namespace EJournalWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuthRepository _authRepository;
        private SettingsRepository _settingsRepository;
        public MainWindow()
        {
            InitializeComponent();
            _authRepository = (App.Current as App).GetAuthRepository;
            _settingsRepository = (App.Current as App).GetSettingsRepository;
            if (_authRepository.IsAuthorized())
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

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is MainPage)
            {
                SettingsMenuItem.IsEnabled = true;
                DateMailSaveMenuItem.IsChecked = _settingsRepository.GetSaveDateTime;
            }
        }

        private void DateMailSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _settingsRepository.SetDateTimeSave();
            _settingsRepository.SaveSettings();
            DateMailSaveMenuItem.IsChecked = _settingsRepository.GetSaveDateTime;
        }
    }
}
