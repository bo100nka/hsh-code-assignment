namespace DataViewer.ViewModel
{
    /// <summary>
    /// Could be useful to have a common base for all view models at some point
    /// </summary>
    public abstract class ViewModelBase : NotifyBase, IDisposable
    {
        public ViewModelBase()
        {
        }

        /// <summary>
        /// A default <see cref="IDisposable"/> implementation.
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
