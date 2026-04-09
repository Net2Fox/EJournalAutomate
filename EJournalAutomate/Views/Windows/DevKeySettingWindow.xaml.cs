using System.Windows;

namespace EJournalAutomate.Views.Windows;

public partial class DevKeySettingWindow : Window
{
    public string DevKey
    {
        get => DevKeyTextBox.Text;
        set
        {
            DevKeyTextBox.Text = value;
        }
    }
    
    public DevKeySettingWindow()
    {
        InitializeComponent();
    }

    private void SetDevKeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(DevKey))
        {
            this.DialogResult = true;
        }
    }
}