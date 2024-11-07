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
        public MainWindow()
        {
            InitializeComponent();
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
            await DataRepository.GetInstance().LoadDataAPI();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is MainPage)
            {
                RedownloadDataMenuItem.IsEnabled = true;
                SettingsMenuItem.IsEnabled = true;
            }
        }
    }
}
