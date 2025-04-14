// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using DataViewer.Models.Exceptions;


namespace DataViewer.Interfaces
{
    public interface IDataParser<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DataParseException">An attempt to parse the data failed or resulted in an underlying exception.</exception>
        T Parse();
    }

}
