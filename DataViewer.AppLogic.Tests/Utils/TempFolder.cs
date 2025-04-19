namespace DataViewer.AppLogic.Tests.Utils
{
    /// <summary>
    /// A temp folder with unique name that will be deleted upon disposal.
    /// </summary>
    internal sealed class TempFolder : IDisposable
    {
        /// <summary>
        /// Creates a new temp directory with a unique name.
        /// </summary>
        public TempFolder()
        {
            FullPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(FullPath);
        }

        /// <summary>
        /// The full path of the created temp directory.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// The <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            if (FullPath == null || !Directory.Exists(FullPath))
                return;
            Directory.Delete(FullPath, true);
        }

        /// <summary>
        /// Useful for debugging or logging.
        /// </summary>
        /// <returns>Full path of the temp directory.</returns>
        public override string ToString() => $"{FullPath}";
    }
}
