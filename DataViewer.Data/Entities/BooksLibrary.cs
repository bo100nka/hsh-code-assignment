namespace DataViewer.Data.Entities
{
    /// <summary>
    /// The root structure of the data to be parsed.
    /// </summary>
    public class BooksLibrary : IEquatable<BooksLibrary>, ICloneable
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
            => Equals(obj as BooksLibrary);

        public bool Equals(BooksLibrary? other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                Version == other.Version &&
                Timestamp == other.Timestamp &&
                AreArticlesEqual(Articles, other.Articles);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Version);
            hashCode.Add(Timestamp);

            if (Articles != null)
            {
                foreach (var article in Articles)
                {
                    hashCode.Add(article);
                }
            }

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            var articles = Articles == null ? string.Empty : string.Join(",", Articles.Select(a => $"{a}"));
            return $"{Version}, {Timestamp}, [{articles}]";
        }

        public object Clone()
        {
            return new BooksLibrary
            {
                Version = Version,
                Timestamp = Timestamp,
                Articles = Articles?.Select(a => (BooksLibraryArticle?)a?.Clone()).ToArray(),
            };
        }

        private static bool AreArticlesEqual(BooksLibraryArticle?[]? articles1, BooksLibraryArticle?[]? articles2)
        {
            if (ReferenceEquals(articles1, articles2))
                return true;

            if (articles1 is null || articles2 is null)
                return articles1 is null
                    && articles2 is null;

            return articles1.SequenceEqual(articles2);
        }
    }
}
