using System.Windows;
using Application = System.Windows.Application;

namespace EJournalAutomate.Services.UI
{
    public class DispatcherService : IDispatcherService
    {
        public Task InvokeOnUIThreadAsync(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return Application.Current.Dispatcher.InvokeAsync(action).Task;
            }
        }
    }
}
