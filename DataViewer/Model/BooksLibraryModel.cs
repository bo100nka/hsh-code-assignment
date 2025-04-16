// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.Models.Data
{
    public class BooksLibraryModel : ModelBase
    {
        private string? _version;
        private string? _timestamp;

        public BooksLibraryModel(BooksLibrary booksLibrary)
        {
            Version = booksLibrary?.Version;
            Timestamp = booksLibrary?.Timestamp;
        }

        public string? Version
        {
            get => _version;
            set => SetAndNotifyIfNewValue(ref _version, value, nameof(Version));
        }

        public string? Timestamp
        {
            get => _timestamp;
            set => SetAndNotifyIfNewValue(ref _timestamp, value, nameof(Timestamp));
        }
    }
}
