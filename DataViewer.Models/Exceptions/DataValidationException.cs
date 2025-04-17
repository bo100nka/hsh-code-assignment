namespace DataViewer.Models.Exceptions
{
    /// <summary>
    /// A dedicated exception used by data validator.
    /// </summary>
    [Serializable]
    public class DataValidationException : Exception
    {
        public DataValidationException()
        {
        }

        public DataValidationException(string? message) : base(message)
        {
        }

        public DataValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
