namespace DataViewer.Models.Data
{
    /// <summary>
    /// The inner structure of each element inside the collection of items stored in the top structure.
    /// </summary>
    public class BooksLibraryArticle : IEquatable<BooksLibraryArticle>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as BooksLibraryArticle);
        }

        public bool Equals(BooksLibraryArticle? other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                Isbn13 == other.Isbn13 &&
                Title == other.Title &&
                Author == other.Author &&
                Language == other.Language &&
                Pages == other.Pages;
        }

        public override int GetHashCode()
        {
            const int primeNumber = 47;
            unchecked // expected to overflow
            {
                var hashCode = Isbn13?.GetHashCode() ?? 0;
                hashCode = (hashCode * primeNumber) ^ (Title?.GetHashCode() ?? 0);
                hashCode = (hashCode * primeNumber) ^ (Author?.GetHashCode() ?? 0);
                hashCode = (hashCode * primeNumber) ^ (Language.GetHashCode());
                hashCode = (hashCode * primeNumber) ^ (Pages?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{(Title == null ? string.Empty : Title.Substring(0, Title.Length > 15 ? 15 : Title.Length))}";
        }
    }
}
