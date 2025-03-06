using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EJournalAutomateMVVM.Services
{
    public interface INavigationService
    {
        void SetFrame(Frame frame);
        void NavigateTo(Type pageType, object parameter = null);
        void GoBack();
    }
}
