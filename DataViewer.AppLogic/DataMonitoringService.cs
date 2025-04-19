using DataViewer.Interfaces;

namespace DataViewer.AppLogic
{
    /// <summary>
    /// A data monitoring service that utilizes external parsing and validating logics.
    /// </summary>
    /// <typeparam name="T">A <see cref="ICloneable"/> and <see cref="IEquatable{T}"/> data type.</typeparam>
    public class DataMonitoringService<T> : IDisposable, IDataMonitoringService<T>
        where T : ICloneable, IEquatable<T>
    {
        private readonly PeriodicTimer _timer;
        private readonly IDataParser<T> _dataParser;
        private readonly IDataValidator<T> _dataValidator;
        private bool _disposed;
        private bool _started;
        private T? _current;
        private T? _source;

        /// <summary>
        /// Constructs the a new instance of a data monitoring service that internally utilizes <see cref="PeriodicTimer"/>.
        /// </summary>
        /// <param name="dataParser">An implementation of a data parser logic. Cannot be null.</param>
        /// <param name="dataValidator">An implementation of a data validation logic. Cannot be null.</param>
        /// <param name="timerInterval">A time interval for the monitoring loop sequence duration. Canno be <see cref="TimeSpan.Zero"/>.</param>
        /// <exception cref="ArgumentException">When <paramref name="timerInterval"/> is <see cref="TimeSpan.Zero"/></exception>
        /// <exception cref="ArgumentNullException">When either <paramref name="dataParser"/> or <paramref name="dataValidator"/> is null.</exception>
        public DataMonitoringService(IDataParser<T> dataParser, IDataValidator<T> dataValidator, TimeSpan timerInterval)
        {
            if (timerInterval == TimeSpan.Zero)
                throw new ArgumentException("Expected a positive time interval value.", nameof(timerInterval));
            _dataParser = dataParser ?? throw new ArgumentNullException(nameof(dataParser));
            _dataValidator = dataValidator ?? throw new ArgumentNullException(nameof(dataValidator));
            _timer = new PeriodicTimer(timerInterval);
            _disposed = false;
        }

        /// <inheritdoc/>
        public T? Source
        {
            get => _source;
            private set
            {
                _source = value;
                DetectChanges();
            }
        }

        /// <inheritdoc/>
        public T? Current
        {
            get => _current;
            private set
            {
                _current = value;
                DetectChanges();
            }
        }

        /// <inheritdoc/>
        public bool DetectedChanges { get; private set; }

        /// <inheritdoc/>
        public Exception? LastException { get; private set; }

        /// <inheritdoc/>
        public bool LastReloadSucceeded { get; private set; }

        /// <summary>
        /// Occurs every time there is an advancement in the monitoring sequence step, which is one of the following:
        /// 1. Data parsing has failed (then <see cref="LastException"/> will be set)
        /// 2. Data validation has failed (then <see cref="LastException"/> will be set)
        /// 3. Parsing and validation succeeded (then <see cref="LastException"/> will be null and <see cref="LastReloadSucceeded"/> will be true).
        /// </summary>
        public event EventHandler? OnProgress;

        /// <inheritdoc/>
        public bool MonitorSourceData()
        {
            LastReloadSucceeded = false;

            if (!TryProgress(() => Source = _dataParser.Parse()))
                return false;

            if (!TryProgress(() => _dataValidator.Validate(Source)))
                return false;

            if (!TryProgress(SetSucceeded, true))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public void PromoteSourceAsCurrent() => Current = (T?)Source?.Clone();

        /// <inheritdoc/>
        public async Task StartMonitoringAsync(CancellationToken cancellationToken)
        {
            ThrowIfAlreadyStarted();

            try
            {
                // calling Dispose() on _timer instead of cancelling a token (and catching exception below) should also stop thread,
                // but i handle this in both ways
                while (await _timer.WaitForNextTickAsync(cancellationToken))
                    MonitorSourceData();
            }
            catch (TaskCanceledException)
            {
                // Expected, albeit it should be sufficient to only catch it's parent type (OperationCanceledException)
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            finally
            {
                _started = false;
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _timer.Dispose();
            _disposed = true;
        }

        private void SetSucceeded()
        {
            LastException = null;
            LastReloadSucceeded = true;
        }

        /// <summary>
        /// cache the changes only when either Current or Source changes so that Equals is not executed as often from the caller
        /// </summary>
        private void DetectChanges() => DetectedChanges = !Equals(Current, Source);

        private bool TryProgress(Action action, bool complete = false)
        {
            try
            {
                action();

                // because the TryProgress helper function is used also in other places,
                // reporting progress on a successful (but not last) step of the monitoring
                // sequence may be considered as "success too early" and thus confusing for the caller.
                if (complete)
                    OnProgress?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                LastException = ex;
                LastReloadSucceeded = false;
                OnProgress?.Invoke(this, EventArgs.Empty);
                return false;
            }
        }

        /// <summary>
        /// The <see cref="PeriodicTimer"/> should only be started once.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ThrowIfAlreadyStarted()
        {
            if (_started)
                throw new InvalidOperationException($"{nameof(StartMonitoringAsync)} was already invoked before.");

            _started = true;
        }
    }
}
