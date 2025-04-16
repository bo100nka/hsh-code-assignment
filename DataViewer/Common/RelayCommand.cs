using System.Windows.Input;

namespace DataViewer.Common
{
    /// <summary>
    /// An ICommand-based class with delegates refered by other object instances
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Predicate<object?>? _canExecute;
        private Action<object?> _execute;

        /// <summary>
        /// Creates a new command that can execute always.
        /// </summary>
        /// <param name="execute">The execution logic (can not be null)</param>
        public RelayCommand(Action<object?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Creates a new command that can execute if canExecute returns true
        /// </summary>
        /// <param name="execute">The execution logic (can not be null)</param>
        /// <param name="canExecute">The execution prevention logic (null = always can execute)</param>
        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
            : this(execute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Allows/denies the execution of this command.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>true when the command can execute, otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Performs the action this command represents.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
