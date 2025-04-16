// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace DataViewer.ViewModel
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetAndNotifyIfNewValue<T>(ref T? field, T? value, string fieldName)
        //where T : class
        {
            if (field?.Equals(value) ?? false)
                return;

            field = value;
            OnPropertyChanged(fieldName);
        }
    }
}
