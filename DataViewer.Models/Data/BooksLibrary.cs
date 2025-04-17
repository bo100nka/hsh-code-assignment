namespace DataViewer.Models.Data
{
    /// <summary>
    /// The root structure of the data to be parsed.
    /// </summary>
    public class BooksLibrary
    {
        /// <summary>
        /// Allow nullable values
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Allow nullable values
        /// </summary>
        public string? Timestamp { get; set; }

        /// <summary>
        /// Allow nullable values, both the array itself and each item in the collection.
        /// </summary>
        public BooksLibraryArticle?[]? Articles { get; set; }
    }
}
