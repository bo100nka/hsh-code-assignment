namespace DataViewer.Data.Exceptions
{
    /// <summary>
    /// A dedicated exception used by data parser.
    /// </summary>
    [Serializable]
    public class DataParseException : Exception
    {
        public DataParseException()
        {
        }

        public DataParseException(string? message) : base(message)
        {
        }

        public DataParseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
