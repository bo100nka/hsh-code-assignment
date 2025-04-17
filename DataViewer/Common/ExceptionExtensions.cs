namespace DataViewer.Common
{
    /// <summary>
    /// A set of helper functions to allow formatting of Exceptions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns the Name of the type of the exception followed by it's message.
        /// If <paramref name="includeStackTrace"/> is `true`, then stack trace follows as well.
        /// Useful for debugging.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="includeStackTrace"></param>
        /// <returns>A string instance summarizing the <paramref name="exception"/>.</returns>
        /// <exception cref="NullReferenceException">When <paramref name="exception"/> is null.</exception>
        public static string FormatException(this Exception exception, bool includeStackTrace = false)
        {
            if (exception == null)
                throw new NullReferenceException(nameof(exception));

            var trace = !includeStackTrace
                ? string.Empty
                : $"\n\nStack trace:\n{exception.StackTrace}";

            return $"{exception.GetType().Name}: {exception.Message}{trace}";
        }
    }
}
