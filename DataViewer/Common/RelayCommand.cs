using System.Windows.Input;

namespace DataViewer.Common
{
    /// <summary>
    /// A basic implementation of the MVVM command binding.
    /// </summary>
    /// <remarks>
    /// Creates a new command that can execute always.
    /// </remarks>
    /// <param name="execute">The execution logic (can not be null)</param>
    /// <exception cref="ArgumentNullException">When <paramref name="execute"/> is null.</exception>
    public class RelayCommand(Action<object?> execute) : ICommand
    {
        private readonly Predicate<object?>? _canExecute;
        private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        /// <summary>
        /// Internally subscribes to <see cref="CommandManager.RequerySuggested"/> event.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new command that can execute if canExecute returns true
        /// </summary>
        /// <param name="execute">The execution logic (can not be null)</param>
        /// <param name="canExecute">The execution prevention logic (null = always can execute)</param>
        /// <exception cref="ArgumentNullException">When <paramref name="execute"/> is null.</exception>
        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
            : this(execute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Allows/denies the execution of this command.
        /// </summary>
        /// <param name="parameter">Optional context parameter.</param>
        /// <returns>true when the command can execute, otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Performs the action this command represents.
        /// </summary>
        /// <param name="parameter">Optional context parameter.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
