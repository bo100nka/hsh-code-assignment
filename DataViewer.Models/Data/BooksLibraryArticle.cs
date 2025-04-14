// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.Models.Data
{
    public class BooksLibraryArticle
    {
        public string? Isbn13 { get; set; }

        public string? Title { get; set; }

        public string? Author { get; set; }

        public BooksLibraryArticleLanguage Language { get; set; }

        public int? Pages { get; set; }
    }
}
