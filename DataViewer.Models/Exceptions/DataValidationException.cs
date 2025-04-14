// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.Models.Exceptions
{
    [Serializable]
    public class DataValidationException : Exception
    {
        public DataValidationException()
        {
        }

        public DataValidationException(string? message) : base(message)
        {
        }

        public DataValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
