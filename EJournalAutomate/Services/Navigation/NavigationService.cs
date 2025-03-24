using System.Windows.Controls;

namespace EJournalAutomate.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        public void SetFrame(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(Type pageType, object parameter = null)
        {
            if (_frame == null)
            {
                throw new InvalidOperationException("Контейнер навигации (Frame) не установлен.");
            }

            var page = (Page)Activator.CreateInstance(pageType);
            if (parameter != null)
            {
                page.DataContext = parameter;
            }
            _frame.Navigate(page);
        }

        public void GoBack()
        {
            if (_frame == null)
            {
                throw new InvalidOperationException("Контейнер навигации (Frame) не установлен.");
            }
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }
    }
}
