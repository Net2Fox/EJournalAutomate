namespace EJournalAutomate.Messages
{
    public class NavigationMessage
    {
        public Type NavigatedPageType { get; }

        public NavigationMessage(Type pageType)
        {
            NavigatedPageType = pageType;
        }
    }
}
