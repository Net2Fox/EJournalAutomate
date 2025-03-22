namespace EJournalAutomateMVVM.Services.UI
{
    public interface IDispatcherService
    {
        Task InvokeOnUIThreadAsync(Action action);
    }
}
