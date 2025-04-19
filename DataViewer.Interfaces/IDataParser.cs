using DataViewer.Data.Exceptions;

namespace DataViewer.Interfaces
{
    /// <summary>
    /// A data parser of a given <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataParser<T>
    {
        /// <summary>
        /// Parses the underlying data, returning the given <typeparamref name="T"/> data type or <see cref="DataParseException"/>
        /// on failure.
        /// </summary>
        /// <returns>A valid instance of parsed <typeparamref name="T"/> data.</returns>
        /// <exception cref="DataParseException">An attempt to parse the data failed or resulted in an underlying exception.</exception>
        T Parse();
    }

}
