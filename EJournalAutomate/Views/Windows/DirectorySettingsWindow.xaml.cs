using System.Windows;

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
