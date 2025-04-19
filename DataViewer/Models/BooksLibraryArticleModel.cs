using DataViewer.Data.Entities;

namespace DataViewer.Models
{
    /// <summary>
    /// A UI Model for representing an <see cref="BooksLibraryArticle"/> object instance.
    /// </summary>
    public class BooksLibraryArticleModel : ModelBase
    {
        private string? _title;
        private string? _isbn13;
        private string? _author;
        private string? _language;
        private int? _pages;

        /// <summary>
        /// A basic string representation of each attribute.
        /// </summary>
        /// <param name="article">Can be null.</param>
        public BooksLibraryArticleModel(BooksLibraryArticle? article)
        {
            Isbn13 = article?.Isbn13;
            Title = article?.Title;
            Author = article?.Author;
            Language = article?.Language.ToString();
            Pages = article?.Pages;
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Isbn13"/> parameter.
        /// </summary>
        public string? Isbn13
        {
            get => _isbn13;
            set => SetAndNotifyIfNewValue(ref _isbn13, value, nameof(Isbn13));
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Title"/> parameter.
        /// </summary>
        public string? Title
        {
            get => _title;
            set => SetAndNotifyIfNewValue(ref _title, value, nameof(Title));
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Author"/> parameter.
        /// </summary>
        public string? Author
        {
            get => _author;
            set => SetAndNotifyIfNewValue(ref _author, value, nameof(Author));
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Language"/> parameter.
        /// </summary>
        public string? Language
        {
            get => _language;
            set => SetAndNotifyIfNewValue(ref _language, value, nameof(Language));
        }

        /// <summary>
        /// Gets, sets and notifies changes about a bound <see cref="Pages"/> parameter.
        /// </summary>
        public int? Pages
        {
            get => _pages;
            set => SetAndNotifyIfNewValue(ref _pages, value, nameof(Pages));
        }
    }
}
