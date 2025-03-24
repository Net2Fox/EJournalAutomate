namespace EJournalAutomateMVVM.Helpers
{
    public class StatusChangeEventArgs : EventArgs
    {
        public string Message { get; }
        public bool IsLoading { get; }

        public StatusChangeEventArgs(string message, bool isLoading)
        {
            Message = message;
            IsLoading = isLoading;
        }
    }
}
