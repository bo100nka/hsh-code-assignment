// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using DataViewer.Models.Exceptions;

namespace DataViewer.Interfaces
{
    public interface IDataValidator<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DataValidationException"></exception>
        void Validate(T value);
    }
}
