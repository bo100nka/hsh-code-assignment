using DataViewer.Data.Entities;
using DataViewer.Data.Exceptions;

namespace DataViewer.AppLogic.Tests
{
    public class BooksLibraryValidatorTests
    {
        [Fact]
        public void Constructor_ConstructsValidInstance()
        {
            Assert.NotNull(new BooksLibraryValidator());
        }

        [Fact]
        public void Validate_WithNewInstance_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = new();

            // Act, Assert
            // with just a new instance, we don't care about precisely which property is missing or invalid
            Assert.Throws<DataValidationException>(() => unit.Validate(instance));
        }

        [Fact]
        public void Validate_WithMissingVersion_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();
            instance.Version = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains(nameof(BooksLibrary.Version), ex.Message);
        }

        [Fact]
        public void Validate_WithMissingTimestamp_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();
            instance.Timestamp = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains(nameof(BooksLibrary.Timestamp), ex.Message);
        }

        [Fact]
        public void Validate_WithMissingArticles_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();
            instance.Articles = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains(nameof(BooksLibrary.Articles), ex.Message);
        }

        [Fact]
        public void Validate_WithEmptyArticles_DoesNotThrowAnyException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();
            instance.Articles = [];

            // Act
            unit.Validate(instance);

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void Validate_WithNullArticle_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.Equal(2, instance.Articles.Length);
            instance.Articles[1] = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] is null", ex.Message);
        }

        [Fact]
        public void Validate_WithArticleWithMissingAuthor_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.NotNull(instance.Articles[1]);
            instance.Articles[1]!.Author = null;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] property {nameof(BooksLibraryArticle.Author)}", ex.Message);
        }

        [Fact]
        public void Validate_WithArticleWithMissingTitle_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.NotNull(instance.Articles[1]);
            instance.Articles[1]!.Title = null;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] property {nameof(BooksLibraryArticle.Title)}", ex.Message);
        }

        [Fact]
        public void Validate_WithArticleWithMissingLanguage_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.NotNull(instance.Articles[1]);
            instance.Articles[1]!.Language = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] property {nameof(BooksLibraryArticle.Language)}", ex.Message);
        }

        [Fact]
        public void Validate_WithArticleWithMissingIsbn13_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.NotNull(instance.Articles[1]);
            instance.Articles[1]!.Isbn13 = null;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] property {nameof(BooksLibraryArticle.Isbn13)}", ex.Message);
        }

        [Fact]
        public void Validate_WithArticleWithMissingPages_ThrowsDataValidationException()
        {
            // Arrange
            var unit = new BooksLibraryValidator();
            BooksLibrary instance = GetValidBooksLibrary();

            Assert.NotNull(instance.Articles);
            Assert.NotNull(instance.Articles[1]);
            instance.Articles[1]!.Pages = default;

            // Act, Assert
            DataValidationException ex = Assert.Throws<DataValidationException>(() => unit.Validate(instance));

            Assert.Contains($"{nameof(BooksLibraryArticle)}[1] property {nameof(BooksLibraryArticle.Pages)}", ex.Message);
        }

        private static BooksLibrary GetValidBooksLibrary()
        {
            BooksLibrary result = new()
            {
                Version = "1.2.3",
                Timestamp = "2025-04-14 16:17",
                Articles =
                [
                    new BooksLibraryArticle
                    {
                        Author = "Author.1",
                        Isbn13 = "isbn-13",
                        Language = BooksLibraryArticleLanguage.Swedish,
                        Pages = 777,
                        Title = "Books title",
                    },
                    new BooksLibraryArticle
                    {
                        Author = "Author.2",
                        Isbn13 = "isbn13",
                        Language = BooksLibraryArticleLanguage.English,
                        Pages = 666,
                        Title = "Title of a book",
                    }
                ]
            };

            return result;
        }
    }
}
