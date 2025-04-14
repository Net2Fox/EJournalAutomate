namespace EJournalAutomate.Exceptions
{
    public class APIException : Exception
    {
        public int? StatusCode { get; }

        public APIException(string message, int? statusCode = null)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public APIException(string message, Exception innerException, int? statusCode = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
