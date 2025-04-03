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

namespace EJournalAutomate.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DirectorySettingsWindow.xaml
    /// </summary>
    public partial class DirectorySettingsWindow : Window
    {
        public string SavePath
        {
            get => CurrentDirTextBox.Text;
            set
            {
                CurrentDirTextBox.Text = value;
            }
        }

        public DirectorySettingsWindow()
        {
            InitializeComponent();
        }

        private void ChangeDirButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog folderDialog = new();
            if (folderDialog.ShowDialog() == true)
            {
                CurrentDirTextBox.Text = folderDialog.FolderName;
                this.DialogResult = true;
            }
        }
    }
}
