// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.ViewModel
{
    public class ViewModelBase : NotifyBase, IDisposable
    {
        public ViewModelBase()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        protected CancellationTokenSource CancellationTokenSource { get; }

        public virtual void Dispose()
        {
            
        }
    }
}
