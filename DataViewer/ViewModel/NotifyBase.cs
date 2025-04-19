using System.ComponentModel;

namespace DataViewer.ViewModel
{
    /// <summary>
    /// The MVVM binding in WPF requires <see cref="INotifyPropertyChanged"/> for broadcasting
    /// changes between UI elements and its underlying view model it is bound to via DataContext.
    /// </summary>
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// MVVM property value update event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Signals the registered subscribers to <see cref="PropertyChanged"/> about the value of
        /// a possibly bound property (<paramref name="propertyName"/>) was just changed.
        /// </summary>
        /// <param name="propertyName">The name of the potentially bound property to broadcast for.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// A helper function to reduce scaffolding of the setters of bound properties.
        /// </summary>
        /// <typeparam name="T">The underlying data type the bound property stored data in.</typeparam>
        /// <param name="field">The bound property's data storage field.</param>
        /// <param name="value">The new value that trigerred the update.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected void SetAndNotifyIfNewValue<T>(ref T? field, T? value, string propertyName)
        {
            if (field?.Equals(value) ?? false)
                return;

            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
