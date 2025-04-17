namespace DataViewer.Models.Data
{
    /// <summary>
    /// The inner structure of each element inside the collection of items stored in the top structure.
    /// </summary>
    public class BooksLibraryArticle
    {
        /// <summary>
        /// Allow nullable values
        /// </summary>
        public string? Isbn13 { get; set; }

        /// <summary>
        /// Allow nullable values
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Allow nullable values
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Default language would be undefined but not nullable.
        /// </summary>
        public BooksLibraryArticleLanguage Language { get; set; }

        /// <summary>
        /// Allow nullable values
        /// </summary>
        public int? Pages { get; set; }
    }
}
