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
        private bool _isDataLoaded = false;
        private int _limit = 20;
        private int _offset = 0;
        private DataRepository _dataRepository;

        public MainPage(List<CefSharp.Cookie> cefSharpCookies = null)
        {
            InitializeComponent();
            DataRepository.Initialize(cefSharpCookies);
            _dataRepository = DataRepository.GetInstance();
            _dataRepository.LoadDataSuccessEvent += LoadData;
            _dataRepository.BeginDataLoadingEvent += DataLoadingProgress;
            _dataRepository.DataLoadingErrorEvent += DataLoadingErrorEvent;
            _dataRepository.DownloadingFinishEvent += DownloadingFinish;
        }

        private void DownloadingFinish()
        {
            Application.Current.Dispatcher.Invoke(() => {
                MessageBox.Show("Вложения из писем успешно скачаны!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(_dataRepository.GetMails());
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
                _isDataLoaded = true;
                Filter();
                LoadingSplashPanel.Visibility = Visibility.Collapsed;
                if (_offset >= _limit)
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
                _isDataLoaded = false;
            });
        }

        private void Filter()
        {
            List<Mail> filteredList = _dataRepository.GetMails();

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
            if (_isDataLoaded == true)
            {
                Filter();
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isDataLoaded == true)
            {
                Filter();
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_offset >= _limit)
            {
                _offset -= _limit;
            }
            await _dataRepository.GetMailsFromAPI(_limit, _offset);
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            _offset += _limit;
            await _dataRepository.GetMailsFromAPI(_limit, _offset);
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
            if (e.Key == System.Windows.Input.Key.Enter && _isDataLoaded && int.TryParse(CountTextBox.Text, out _limit))
            {
                await _dataRepository.GetMailsFromAPI(_limit);
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
            _dataRepository.DownloadFile(_dataRepository.GetMails().Where(m => m.IsSelected == true && m.HasFiles == true).ToList());
        }
    }
}
