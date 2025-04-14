namespace EJournalAutomate.Messages
{
    public class DataLoadMessage
    {
        public string Message { get; }
        public bool IsLoading { get; }

        public DataLoadMessage(string message, bool isLoading)
        {
            Message = message;
            IsLoading = isLoading;
        }

        public DataLoadMessage(bool isLoading)
        {
            Message = string.Empty;
            IsLoading = isLoading;
        }
    }
}
