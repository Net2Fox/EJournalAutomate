using Microsoft.Extensions.Logging;
using System.Windows.Controls;

namespace EJournalAutomate.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        private readonly ILogger<NavigationService> _logger;

        public NavigationService(ILogger<NavigationService> logger)
        {
            _logger = logger;

            _logger.LogInformation("NavigationService инициализирован");
        }


        public void SetFrame(Frame frame)
        {
            _logger.LogInformation("Попытка установить Frame (SetFrame)");

            if (frame == null)
            {
                var ex = new ArgumentNullException(nameof(frame));
                _logger.LogCritical(ex, "SetFrame прошёл неудачно");
                throw ex;
            }
            else
            {
                _frame = frame;
                _logger.LogInformation("SetFrame прошёл удачно");
            }
        }

        public void NavigateTo(Type pageType, object parameter = null)
        {
            _logger.LogInformation($"Попытка перейти на страницу: {pageType}");

            if (_frame == null)
            {
                var ex = new InvalidOperationException("Контейнер навигации (Frame) не установлен.");
                _logger.LogError(ex, $"Переход на страницу не удался: {pageType}");
                throw ex;
            }

            var page = (Page)Activator.CreateInstance(pageType);

            if (parameter != null)
            {
                page.DataContext = parameter;
            }

            _frame.Navigate(page);

            _logger.LogInformation($"Переход на страницу прошёл успешно: {pageType}");
        }

        public void GoBack()
        {
            _logger.LogInformation("Попытка перейти на страницу назад");

            if (_frame == null)
            {
                var ex = new InvalidOperationException("Контейнер навигации (Frame) не установлен.");
                _logger.LogCritical(ex, "Переход на страницу назад не удался");
                throw ex;
            }
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
                _logger.LogInformation("Переход на страницу назад прошёл успешно");
            }
        }
    }
}
