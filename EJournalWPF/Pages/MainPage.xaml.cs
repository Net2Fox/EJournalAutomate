using EJournalWPF.Data;
using EJournalWPF.Model;
using EJournalWPF.Model.API.MessageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private DataRepository _dataRepository;

        public MainPage()
        {
            InitializeComponent();
            _dataRepository = DataRepository.GetInstance();
            _dataRepository.GetMessagesEvent += GetMessagesEvent;

            Task.Run(async () =>
            {
                await _dataRepository.GetMessages();
            });
        }

        private void GetMessagesEvent(bool isSuccess, List<Message> messages, string error)
        {
            Application.Current.Dispatcher.Invoke(() => {
                LoadData(messages);
            });
        }

        private void LoadData(List<Message> messages)
        {
            EmailListBox.ItemsSource = messages;
            EmailListBox.Items.Refresh();
            _isDataLoaded = true;
            Filter();
            LoadingSplashPanel.Visibility = Visibility.Collapsed;
        }

        //private void DownloadingFinish()
        //{
        //    Application.Current.Dispatcher.Invoke(() => {
        //        MessageBox.Show("Вложения из писем успешно скачаны!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //        LoadData(_dataRepository.GetMails());
        //    });
        //}

        //private void DataLoadingErrorEvent(string errorMsg)
        //{
        //    Application.Current.Dispatcher.Invoke(() => {
        //        LoadingSplashPanel.Visibility = Visibility.Visible;
        //        LoadingTextBlock.Text = errorMsg;
        //    });
        //}

        //private void DataLoadingProgress(string message)
        //{
        //    Application.Current.Dispatcher.Invoke(() => {
        //        LoadingTextBlock.Text = message;
        //        LoadingSplashPanel.Visibility = Visibility.Visible;
        //        _isDataLoaded = false;
        //    });
        //}

        private void Filter()
        {
            List<Message> filteredList = _dataRepository.Messages;

            

            if (SearchTextBox.Text != string.Empty && SearchTextBox.Text != "Поиск")
            {
                string text = SearchTextBox.Text.ToLower();
                filteredList = filteredList.Where(m =>
                m.User_From.FirstName.ToLower().Contains(text)
                || m.User_From.LastName.ToLower().Contains(text)
                || m.User_From.MiddleName.ToLower().Contains(text)
                || m.Subject.ToLower().Contains(text)).ToList();
            }

            if (filteredList.Count != 0 && StatusComboBox.SelectedIndex != 0)
            {
                filteredList = filteredList.Where(m => m.Unread == (StatusComboBox.SelectedIndex == 1 ? true : false)).ToList();
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
                await _dataRepository.GetMessages(limit: _limit);
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mail in EmailListBox.Items.SourceCollection as List<Message>)
            {
                mail.Selected = true;
            }
            EmailListBox.Items.Refresh();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            _dataRepository.DownloadFile(_dataRepository.Messages.Where(m => m.Selected == true && m.With_Files == true).ToList());
        }
    }
}
