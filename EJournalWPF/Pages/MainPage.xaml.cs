using EJournalWPF.Data;
using EJournalWPF.Model;
using EJournalWPF.Model.API.MessageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
            _dataRepository = (App.Current as App).GetDataRepository;
            
            _dataRepository.GetMessagesEvent += GetMessagesEvent;

            _dataRepository.DownloadMessagesEvent += DownloadMessagesEvent;

            LoadingSplashPanel.Visibility = Visibility.Visible;

            Task.Run(async () =>
            {
                await InitializeData();
            });
        }

        

        private async Task InitializeData()
        {
            
            bool result = await _dataRepository.Initialize();
            if (result)
            {
                await _dataRepository.GetMessages();
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    LoadingTextBlock.Text = "При инициализации данных произошла ошибка. Перезапустите программу!";
                });
            }
        }

        private void GetMessagesEvent(bool isSuccess, List<Model.API.MessageModel.Message> messages, string error)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                LoadData(messages);
            });
        }

        private void DownloadMessagesEvent(bool isSuccess, string error)
        {
            if (isSuccess)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    System.Windows.MessageBox.Show("Вложения из писем успешно скачаны!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData(_dataRepository.Messages);
                });
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    LoadingSplashPanel.Visibility = Visibility.Visible;
                    LoadingTextBlock.Text = error;
                });
            }
        }

        private void LoadData(List<Model.API.MessageModel.Message> messages)
        {
            EmailListBox.ItemsSource = messages;
            EmailListBox.Items.Refresh();
            _isDataLoaded = true;
            Filter();
            LoadingSplashPanel.Visibility = Visibility.Collapsed;
        }

        private void Filter()
        {
            List<Model.API.MessageModel.Message> filteredList = _dataRepository.Messages;

            if (SearchTextBox.Text != string.Empty && SearchTextBox.Text != "Поиск")
            {
                string text = SearchTextBox.Text.ToLower();
                filteredList = filteredList.Where(m =>
                m.User_From.FirstName.ToLower().Contains(text)
                || m.User_From.LastName.ToLower().Contains(text)
                || m.User_From.MiddleName.ToLower().Contains(text)
                || m.Subject.ToLower().Contains(text)).ToList();
            }

            if (filteredList?.Count != 0 && StatusComboBox.SelectedIndex != 0)
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
                LoadingTextBlock.Text = $"Загрузка данных, пожалуйста подождите...";
                LoadingSplashPanel.Visibility = Visibility.Visible;
                _isDataLoaded = false;
                await _dataRepository.GetMessages(limit: _limit);
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var message in EmailListBox.Items.SourceCollection as List<Model.API.MessageModel.Message>)
            {
                message.Selected = true;
            }
            EmailListBox.Items.Refresh();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var messagesToDownlaod = _dataRepository.Messages.Where(m => m.Selected == true && m.With_Files == true).ToList();

            LoadingTextBlock.Text = $"Скачивание {messagesToDownlaod.Count} сообщений...";
            LoadingSplashPanel.Visibility = Visibility.Visible;
            _isDataLoaded = false;

            _dataRepository.DownloadFile(messagesToDownlaod);
        }
    }
}
