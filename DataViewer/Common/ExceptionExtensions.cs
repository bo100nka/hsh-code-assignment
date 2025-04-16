namespace DataViewer.Common
{
    public static class ExceptionExtensions
    {
        public static string FormatException(this Exception exception, bool includeStackTrace = false)
        {
            var trace = !includeStackTrace ? string.Empty : $"\n\nStack trace:\n{exception.StackTrace}";
            return string.Format("{0}: {1}{2}", exception.GetType().FullName, exception.Message, trace);
        }
    }
}
