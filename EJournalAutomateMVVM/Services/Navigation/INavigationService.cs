using System.Windows.Controls;

namespace EJournalAutomateMVVM.Services.Navigation
{
    public interface INavigationService
    {
        void SetFrame(Frame frame);
        void NavigateTo(Type pageType, object parameter = null);
        void GoBack();
    }
}
