namespace DataViewer.Models.Data
{
    /// <summary>
    /// The root structure of the data to be parsed.
    /// </summary>
    public class BooksLibrary : IEquatable<BooksLibrary>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as BooksLibrary);
        }

        public bool Equals(BooksLibrary? other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                Version == other.Version &&
                Timestamp == other.Timestamp &&

                // Both articles being null should still mean they are equal
                (Articles == other.Articles
                || (
                    // bot not if either of them being null
                    Articles != null && other.Articles != null &&
                    Articles.SequenceEqual(other.Articles)
                ));
        }

        public override int GetHashCode()
        {
            const int primeNumber = 31;
            unchecked // expected to overflow
            {
                var hashCode = Version?.GetHashCode() ?? 0;
                hashCode = (hashCode * primeNumber) ^ (Timestamp?.GetHashCode() ?? 0);

                if (Articles != null)
                {
                    foreach (BooksLibraryArticle? article in Articles)
                    {
                        hashCode = (hashCode * primeNumber) ^ (article?.GetHashCode() ?? 0);
                    }
                }

                return hashCode;
            }
        }

        public override string ToString()
        {
            var articles = Articles == null ? string.Empty : string.Join(",", Articles.Select(a => $"{a}"));
            return $"{Version}, {Timestamp}, [{articles}]";
        }
    }
}
