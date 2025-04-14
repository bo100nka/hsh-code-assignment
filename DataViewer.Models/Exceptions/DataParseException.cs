// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.Models.Exceptions
{
    [Serializable]
    public class DataParseException : Exception
    {
        public DataParseException()
        {
        }

        public DataParseException(string? message) : base(message)
        {
        }

        public DataParseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
