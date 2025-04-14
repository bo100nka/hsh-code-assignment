// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DataViewer.AppLogic.Tests.Utils
{
    internal class TempFolder : IDisposable
    {
        public TempFolder()
        {
            FullPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(FullPath);
        }

        public string FullPath { get; }

        public void Dispose()
        {
            if (FullPath == null || !Directory.Exists(FullPath))
                return;
            Directory.Delete(FullPath, true);
        }

        public override string ToString() => $"{FullPath}";
    }
}
