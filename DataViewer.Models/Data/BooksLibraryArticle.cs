namespace DataViewer.Models.Data
{
    /// <summary>
    /// The inner structure of each element inside the collection of items stored in the top structure.
    /// </summary>
    public class BooksLibraryArticle : IEquatable<BooksLibraryArticle>, ICloneable
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

        public object Clone()
        {
            return new BooksLibraryArticle
            {
                Title = Title,
                Author = Author,
                Isbn13 = Isbn13,
                Language = Language,
                Pages = Pages,
            };
        }

        public override bool Equals(object? obj)
            => Equals(obj as BooksLibraryArticle);

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
            => HashCode.Combine(Isbn13, Title, Author, Language, Pages);

        public override string ToString()
            => $"{Title?.Substring(0, Math.Max(15, Title.Length))}";
    }
}
