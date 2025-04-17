using DataViewer.ViewModel;

namespace DataViewer.Models
{
    /// <summary>
    /// Could be useful to have a common base for all models at some point
    /// </summary>
    public abstract class ModelBase : NotifyBase, IDisposable
    {
        public ModelBase()
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
