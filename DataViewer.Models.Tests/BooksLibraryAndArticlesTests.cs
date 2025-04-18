// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DataViewer.Models.Data;

namespace DataViewer.Models.Tests
{
    public class BooksLibraryAndArticlesTests
    {
        [Fact]
        public void ctor_CreatesEmptyInstance()
        {
            var library = new BooksLibrary();

            Assert.NotNull(library);
            Assert.Null(library.Version);
            Assert.Null(library.Timestamp);
            Assert.Null(library.Articles);
        }

        [Fact]
        public void Equals_HandlesNulls()
        {
            var valid1 = new BooksLibrary();
            var valid2 = new BooksLibrary();
            var invalid = default(BooksLibrary);

            Assert.False(Equals(valid1, invalid));
            Assert.False(Equals(invalid, valid1));
            Assert.True(Equals(invalid, invalid));
            Assert.True(Equals(valid1, valid1));

            Assert.False(valid1.Equals(invalid));
            Assert.True(valid1.Equals(valid1));
            Assert.True(valid1.Equals(valid2));

            Assert.False(StrongEquals(valid1, invalid));
            Assert.True(StrongEquals(valid1, valid1));
            Assert.True(StrongEquals(valid1, valid2));
        }

        [Fact]
        public void Equals_WhenDifferentVersion_EqualsFalse()
        {
            var valid1 = new BooksLibrary { };
            var valid2 = new BooksLibrary { };

            Assert.Equal(valid1, valid2);

            valid1.Version = "same";
            valid2.Version = "same";

            Assert.Equal(valid1, valid2);

            valid2.Version = "different";

            Assert.NotEqual(valid1, valid2);
        }

        [Fact]
        public void Equals_WhenDifferentTimestamp_EqualsFalse()
        {
            var valid1 = new BooksLibrary { };
            var valid2 = new BooksLibrary { };

            Assert.Equal(valid1, valid2);

            valid1.Timestamp = "same";
            valid2.Timestamp = "same";

            Assert.Equal(valid1, valid2);

            valid2.Timestamp = "different";

            Assert.NotEqual(valid1, valid2);
        }

        [Fact]
        public void Equals_WhenDifferentArticles_EqualsFalse()
        {
            var valid1 = new BooksLibrary { };
            var valid2 = new BooksLibrary { };
            var valid3 = new BooksLibrary { };
            var valid4 = new BooksLibrary { };

            Assert.Equal(valid1, valid2);
            Assert.Equal(valid1, valid3);
            Assert.Equal(valid1, valid4);

            valid2.Articles = [];
            valid3.Articles = [default];
            valid4.Articles = [new BooksLibraryArticle { }];

            Assert.NotEqual(valid1, valid2);
            Assert.NotEqual(valid1, valid3);
            Assert.NotEqual(valid1, valid4);
            Assert.NotEqual(valid2, valid3);
            Assert.NotEqual(valid2, valid4);
            Assert.NotEqual(valid3, valid4);

            valid3.Articles = [
                new BooksLibraryArticle { Title = "piglet", Isbn13 = "i1" },
                new BooksLibraryArticle { Title = "elephant", Isbn13 = "i2" }];

            valid4.Articles = [
                new BooksLibraryArticle { Title = "piglet", Isbn13 = "i1" },
                new BooksLibraryArticle { Title = "elephant", Isbn13 = "i2" }];

            Assert.Equal(valid3, valid4);

            SwitchArticlesAtIndices(valid4, 0, 1);

            Assert.NotEqual(valid3, valid4);
        }

        // [...,A,B,...] -> [...,B,A,...]
        private static void SwitchArticlesAtIndices(BooksLibrary instance, int index1, int index2)
        {
            Assert.NotNull(instance.Articles);

            (instance.Articles[index2], instance.Articles[index1]) =
            (instance.Articles[index1], instance.Articles[index2]);
        }

        private static bool StrongEquals<T>(T left, T? right)
            where T : IEquatable<T>
        {
            return left.Equals(right);
        }
    }
}
