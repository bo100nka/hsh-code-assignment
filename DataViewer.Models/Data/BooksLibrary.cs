// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.Models.Data
{
    public class BooksLibrary
    {
        public string? Version { get; set; }

        public string? Timestamp { get; set; }

        public BooksLibraryArticle?[]? Articles { get; set; }
    }
}
