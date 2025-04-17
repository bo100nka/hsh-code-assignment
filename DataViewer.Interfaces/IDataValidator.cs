using DataViewer.Models.Exceptions;

namespace DataViewer.Interfaces
{
    /// <summary>
    /// A validator of <typeparamref name="T"/> data.
    /// </summary>
    /// <typeparam name="T">The given data type of an object instance to validate.</typeparam>
    public interface IDataValidator<T>
    {
        /// <summary>
        /// Validates <paramref name="value"/> and throws <see cref="DataValidationException"/> on validation errors.
        /// </summary>
        /// <param name="value">The object instance which to validate.</param>
        /// <exception cref="DataValidationException">When validation of <paramref name="value"/> failed.</exception>
        void Validate(T value);
    }
}
