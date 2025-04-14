// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using DataViewer.Interfaces;
using DataViewer.Models.Data;
using DataViewer.Models.Exceptions;

namespace DataViewer.AppLogic
{
    public sealed class BooksLibraryValidator : IDataValidator<BooksLibrary>
    {
        public void Validate(BooksLibrary value)
        {
            if (value == null)
                throw new DataValidationException($"The object instance is null.");

            if (string.IsNullOrWhiteSpace(value.Version))
                throw new DataValidationException($"The property {nameof(BooksLibrary.Version)} is null or a whitespace.");

            if (value.Timestamp == null)
                throw new DataValidationException($"The property {nameof(BooksLibrary.Timestamp)} is null.");

            if (!DateTime.TryParseExact(value.Timestamp, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                throw new DataValidationException($"The property {nameof(BooksLibrary.Timestamp)} has invalid format (Expected format: yyyy-MM-dd HH:mm).");

            if (value.Articles == null)
                throw new DataValidationException($"The property {nameof(BooksLibrary.Articles)} is null.");

            // allowing empty list, but not null, because the null is sometimes meaning as omitted or forgotten, or a typo in a name
            if (value.Articles.Length > 0)
            {
                int row = 0;
                foreach (BooksLibraryArticle? article in value.Articles)
                {
                    if (article == null)
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] is null.");

                    // the validation of the following properties could have been more thorough using regex and size limiting but im satisfied with only a basic validation
                    if (string.IsNullOrWhiteSpace(article.Isbn13))
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] property {nameof(BooksLibraryArticle.Isbn13)} is null or a whitespace.");

                    if (string.IsNullOrWhiteSpace(article.Author))
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] property {nameof(BooksLibraryArticle.Author)} is null or a whitespace.");

                    if (string.IsNullOrWhiteSpace(article.Title))
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] property {nameof(BooksLibraryArticle.Title)} is null or a whitespace.");

                    if (article.Language == BooksLibraryArticleLanguage.Undefined)
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] property {nameof(BooksLibraryArticle.Language)} is {nameof(BooksLibraryArticleLanguage.Undefined)}.");

                    if (article.Pages == null)
                        throw new DataValidationException($"The {nameof(BooksLibraryArticle)}[{row}] property {nameof(BooksLibraryArticle.Pages)} is null");

                    row++;
                }
            }
        }
    }
}
