using EJournalAutomate.Data;
using EJournalWPF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EJournalWPF
{
    /// <summary>
    /// Логика взаимодействия для DirectorySettingsWindow.xaml
    /// </summary>
    public partial class DirectorySettingsWindow : Window
    {
        private SettingsRepository _settingsRepository;

        public DirectorySettingsWindow()
        {
            InitializeComponent();
            _settingsRepository = (App.Current as App).GetSettingsRepository;
            CurrentDirTextBox.Text = _settingsRepository.GetSaveDirectory;
        }

        private void ChangeDirButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settingsRepository.SetSaveDirectory(folderBrowserDialog.SelectedPath);
                CurrentDirTextBox.Text = folderBrowserDialog.SelectedPath;
                _settingsRepository.SaveSettings();
                System.Windows.MessageBox.Show("Путь успешно установлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
