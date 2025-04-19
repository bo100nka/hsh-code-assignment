using DataViewer.AppLogic.Tests.Utils;
using DataViewer.Data.Entities;
using DataViewer.Data.Exceptions;
using Xunit.Abstractions;

namespace DataViewer.AppLogic.Tests
{
    public class BooksFileJsonReaderTests(ITestOutputHelper testOutputHelper)
    {
        [Fact]
        public void Constructor_ValidatesArguments()
        {
            Assert.Throws<ArgumentException>(() => new BooksLibraryJsonFileParser(booksFileFullPath: string.Empty));
            Assert.Throws<ArgumentException>(() => new BooksLibraryJsonFileParser(booksFileFullPath: " "));

            var unit = new BooksLibraryJsonFileParser("valid-but-not-existing-file.txt");

            Assert.NotNull(unit);
        }

        [Theory]
        [InlineData(false, "", null)]
        [InlineData(true, "test.txt", "")]
        [InlineData(true, "test.txt", "marry had a little lamb")]
        [InlineData(true, "test.txt", @"{""root"": 123, ""name"": ""value""}")]
        public void ParseAndValidate_WhenAnyExceptionThrown_CatchesAndRethrowsCustomExceptionInstead(bool fileShouldExists, string fileName, string? contents)
        {
            // Arrange
            using var tempFolder = new TempFolder();
            string sourceFileName = "non-existing-file.txt";
            if (fileShouldExists)
                sourceFileName = CreateTempfile(tempFolder.FullPath, fileName, contents);
            var unit = new BooksLibraryJsonFileParser(sourceFileName);

            // Act, Assert
            var exc = default(Exception?);
            var actual = default(BooksLibrary);
            try
            {
                actual = unit.Parse();
            }
            catch (Exception ex)
            {
                exc = ex;
                LogException(ex);
            }

            if (exc != null)
                Assert.IsType<DataParseException>(exc);
            else
            {
                Assert.NotNull(actual);
                Assert.Null(actual.Articles);
                Assert.Null(actual.Version);
                Assert.Null(actual.Timestamp);
            }
        }

        [Fact]
        public void ParseAndValidate_WithValidSourceFile_ReturnsValidBookLibraryInstance()
        {
            // Arrange
            using var tempFolder = new TempFolder();
            string sourceFileName = CreateTempfile(tempFolder.FullPath, "data.json", GetValidBookFileContents());
            var unit = new BooksLibraryJsonFileParser(sourceFileName);

            // Act
            BooksLibrary actual = unit.Parse();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("0.9.1", actual.Version);
            Assert.Equal("2025-04-11 21:15", actual.Timestamp);
            Assert.NotNull(actual.Articles);
            Assert.Equal(2, actual.Articles.Length);

            Assert.Equal("978-0590353427", actual.Articles[0]!.Isbn13);
            Assert.Equal("Harry Potter and the Sorcerer's Stone", actual.Articles[0]!.Title);
            Assert.Equal("J.K. Rowling", actual.Articles[0]!.Author);
            Assert.Equal(309, actual.Articles[0]!.Pages);
            Assert.Equal(BooksLibraryArticleLanguage.English, actual.Articles[0]!.Language);

            Assert.Equal("978-1338299151", actual.Articles[1]!.Isbn13);
            Assert.Equal("Harry Potter and the Chamber of Secrets", actual.Articles[1]!.Title);
            Assert.Equal("J.K. Rowling", actual.Articles[1]!.Author);
            Assert.Equal(368, actual.Articles[1]!.Pages);
            Assert.Equal(BooksLibraryArticleLanguage.English, actual.Articles[1]!.Language);
        }

        private static string GetValidBookFileContents()
        {
            return @"
{
    ""version"": ""0.9.1"",
    ""timestamp"": ""2025-04-11 21:15"",
    ""comment"": ""something useless and ignored here"",
    ""articles"": [
        {
            ""isbn13"": ""978-0590353427"",
            ""title"": ""Harry Potter and the Sorcerer's Stone"",
            ""author"": ""J.K. Rowling"",
            ""pages"": 309,
            ""language"": ""English"",
            ""comment"": ""something useless and ignored here""
        },
        {
            ""isbn13"": ""978-1338299151"",
            ""title"": ""Harry Potter and the Chamber of Secrets"",
            ""author"": ""J.K. Rowling"",
            ""pages"": 368,
            ""language"": ""English"",
            ""comment"": ""something useless and ignored here""
        },
    ]
}
";
        }

        private void LogException(Exception ex)
        {
            testOutputHelper.WriteLine($"Exception: {ex.GetType().FullName} - {ex.Message}");
            testOutputHelper.WriteLine($"Inner: {ex.InnerException?.GetType().FullName} - {ex.InnerException?.Message}");
        }

        private static string CreateTempfile(string tempFolder, string fileName, string? contents)
        {
            string tempFilePath = Path.Combine(tempFolder, fileName);
            File.WriteAllText(tempFilePath, contents);
            return tempFilePath;
        }
    }
}
