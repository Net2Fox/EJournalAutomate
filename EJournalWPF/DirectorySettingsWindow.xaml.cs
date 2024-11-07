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
        public DirectorySettingsWindow()
        {
            InitializeComponent();
            CurrentDirTextBox.Text = DataRepository.GetInstance().SaveDirectory;
        }

        private void ChangeDirButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataRepository.GetInstance().SetSaveDirectory(folderBrowserDialog.SelectedPath);
                CurrentDirTextBox.Text = folderBrowserDialog.SelectedPath;
                System.Windows.MessageBox.Show("Путь успешно установлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
