using EJournalWPF.Data;
using EJournalWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EJournalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private bool isDataLoaded = false;
        private int limit = 20;
        private int offset = 0;
        private DataRepository repository;

        public MainPage(List<CefSharp.Cookie> cefSharpCookies)
        {
            InitializeComponent();
            DataRepository.Initialize(cefSharpCookies);
            repository = DataRepository.GetInstance();
            repository.LoadDataSuccessEvent += LoadData;
            repository.BeginDataLoadingEvent += DataLoadingProgress;
            repository.DataLoadingErrorEvent += DataLoadingErrorEvent;
            repository.DownloadingFinishEvent += DownloadingFinish;
        }

        private void DownloadingFinish()
        {
            Application.Current.Dispatcher.Invoke(() => {
                MessageBox.Show("Вложения из писем успешно скачаны!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(repository.GetMails());
            });
        }

        private void DataLoadingErrorEvent(string errorMsg)
        {
            Application.Current.Dispatcher.Invoke(() => {
                LoadingSplashPanel.Visibility = Visibility.Visible;
                LoadingTextBlock.Text = errorMsg;
            });
        }

        private void LoadData(List<Mail> mails)
        {
            Application.Current.Dispatcher.Invoke(() => {
                EmailListBox.ItemsSource = mails;
                EmailListBox.Items.Refresh();
                isDataLoaded = true;
                Filter();
                LoadingSplashPanel.Visibility = Visibility.Collapsed;
                if (offset >= limit)
                {
                    BackButton.IsEnabled = true;
                } 
                else
                {
                    BackButton.IsEnabled = false;
                }
            });
        }

        private void DataLoadingProgress(string message)
        {
            Application.Current.Dispatcher.Invoke(() => {
                LoadingTextBlock.Text = message;
                LoadingSplashPanel.Visibility = Visibility.Visible;
                isDataLoaded = false;
            });
        }

        private void Filter()
        {
            List<Mail> filteredList = repository.GetMails();

            if (SearchTextBox.Text != string.Empty && SearchTextBox.Text != "Поиск")
            {
                string text = SearchTextBox.Text.ToLower();
                filteredList = filteredList.Where(m =>
                m.FromUser.FirtsName.ToLower().Contains(text)
                || m.FromUser.LastName.ToLower().Contains(text)
                || m.FromUser.MiddleName.ToLower().Contains(text)
                || m.Subject.ToLower().Contains(text)).ToList();
            }

            if (filteredList.Count != 0 && StatusComboBox.SelectedIndex != 0)
            {
                filteredList = filteredList.Where(m => ((int)m.Status) == StatusComboBox.SelectedIndex).ToList();
            }

            EmailListBox.ItemsSource = filteredList;
            EmailListBox.Items.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDataLoaded == true)
            {
                Filter();
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isDataLoaded == true)
            {
                Filter();
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (offset >= limit)
            {
                offset -= limit;
            }
            await repository.GetMailsFromAPI(limit, offset);
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            await repository.GetMailsFromAPI(limit, offset);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск")
            {
                SearchTextBox.Text = string.Empty;
            }
        }

        private void CountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CountTextBox.Text == "Количество писем (по умолчанию 20)")
            {
                CountTextBox.Text = string.Empty;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == string.Empty)
            {
                SearchTextBox.Text = "Поиск";
            }
        }

        private void CountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CountTextBox.Text == string.Empty)
            {
                CountTextBox.Text = "Количество писем (по умолчанию 20)";
            }
        }

        private async void CountTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && isDataLoaded && int.TryParse(CountTextBox.Text, out limit))
            {
                await repository.GetMailsFromAPI(limit);
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mail in EmailListBox.Items.SourceCollection as List<Mail>)
            {
                mail.IsSelected = true;
            }
            EmailListBox.Items.Refresh();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            repository.DownloadFile(repository.GetMails().Where(m => m.IsSelected == true && m.HasFiles == true).ToList());
        }
    }
}
