// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace DataViewer.Models.Data
{
    public class BooksLibraryArticleModel : ModelBase
    {
        private string? _title;
        private string? _isbn13;
        private string? _author;
        private string? _language;
        private int? _pages;

        public BooksLibraryArticleModel(BooksLibraryArticle? article)
        {
            Isbn13 = article?.Isbn13;
            Title = article?.Title;
            Author = article?.Author;
            Language = article?.Language.ToString();
            Pages = article?.Pages;
        }

        public string? Isbn13
        {
            get => _isbn13;
            set => SetAndNotifyIfNewValue(ref _isbn13, value, nameof(Isbn13));
        }

        public string? Title
        {
            get => _title;
            set => SetAndNotifyIfNewValue(ref _title, value, nameof(Title));
        }

        public string? Author
        {
            get => _author;
            set => SetAndNotifyIfNewValue(ref _author, value, nameof(Author));
        }

        public string? Language
        {
            get => _language;
            set => SetAndNotifyIfNewValue(ref _language, value, nameof(Language));
        }

        public int? Pages
        {
            get => _pages;
            set => SetAndNotifyIfNewValue(ref _pages, value, nameof(Pages));
        }
    }
}
