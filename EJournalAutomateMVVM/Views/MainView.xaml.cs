using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace EJournalAutomateMVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetService<MainViewModel>() 
                ?? throw new InvalidOperationException("Не удалось получить MainViewModel из DI контейнера.");
        }
    }
}
