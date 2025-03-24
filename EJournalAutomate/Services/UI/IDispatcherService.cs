namespace EJournalAutomate.Services.UI
{
    public interface IDispatcherService
    {
        Task InvokeOnUIThreadAsync(Action action);
    }
}
