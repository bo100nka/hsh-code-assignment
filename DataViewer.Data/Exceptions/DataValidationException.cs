namespace DataViewer.Data.Exceptions
{
    /// <summary>
    /// A dedicated exception used by data validator.
    /// </summary>
    /// <remarks>
    /// Could have added an explicit PropertyName parameter for easier error reporting but decided to keep it just basic.
    /// </remarks>
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
