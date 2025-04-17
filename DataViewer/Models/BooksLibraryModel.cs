using DataViewer.Models.Data;

namespace DataViewer.Models
{
    /// <summary>
    /// A UI Model for representing an <see cref="BooksLibrary"/> object instance.
    /// </summary>
    public class BooksLibraryModel : ModelBase
    {
        private string? _version;
        private string? _timestamp;

        /// <summary>
        /// A basic string representation of each attribute.
        /// </summary>
        /// <param name="booksLibrary">Can be null.</param>
        public BooksLibraryModel(BooksLibrary? booksLibrary)
        {
            Version = booksLibrary?.Version;
            Timestamp = booksLibrary?.Timestamp;
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Version"/> parameter.
        /// </summary>
        public string? Version
        {
            get => _version;
            set => SetAndNotifyIfNewValue(ref _version, value, nameof(Version));
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Timestamp"/> parameter.
        /// </summary>
        public string? Timestamp
        {
            get => _timestamp;
            set => SetAndNotifyIfNewValue(ref _timestamp, value, nameof(Timestamp));
        }
    }
}
