using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EJournalAutomateMVVM.Services
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
