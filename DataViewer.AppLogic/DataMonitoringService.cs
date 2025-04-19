using DataViewer.Interfaces;

namespace DataViewer.AppLogic
{
    /// <summary>
    /// A data parsing, validating and monitoring service.
    /// </summary>
    /// <typeparam name="T">A <see cref="ICloneable"/> and <see cref="IEquatable{T}"/> data type.</typeparam>
    public class DataMonitoringService<T> : IDisposable
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
        /// <param name="dataParser">An implementation of a data parser logic.</param>
        /// <param name="dataValidator">An implementation of a data validation logic.</param>
        /// <param name="timerInterval">A time interval for the monitoring loop sequence duration.</param>
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

        /// <summary>
        /// Occurs every time there is an advancement in the monitoring sequence step, which is one of the following:
        /// 1. Data parsing has failed (then <see cref="LastException"/> will be set)
        /// 2. Data validation has failed (then <see cref="LastException"/> will be set)
        /// 3. Parsing and validation succeeded (then <see cref="LastException"/> will be null and <see cref="LastReloadSucceeded"/> will be true).
        /// </summary>
        public event EventHandler? OnProgress;

        /// <summary>
        /// Gets whether there are changes between <see cref="Current"/> and <see cref="Source"/>.
        /// </summary>
        public bool DetectedChanges { get; private set; }

        /// <summary>
        /// Gets whether the latest attempt to reload data was successful. <seealso cref="LastException"/>.
        /// </summary>
        public bool LastReloadSucceeded { get; private set; }

        /// <summary>
        /// Holds the Current data instance promoted from <see cref="Source"/>
        /// earlier by calling <see cref="PromoteSourceAsCurrent"/>.
        /// The <see cref="DetectedChanges"/> compares <see cref="Current"/> against <see cref="Source"/>
        /// </summary>
        public T? Current
        {
            get => _current;
            private set
            {
                _current = value;
                DetectChanges();
            }
        }

        /// <summary>
        /// Holds the latest Source data that the <see cref="Current"/> is compared against.
        /// Call the <see cref="PromoteSourceAsCurrent"/> to make the Source become the new <see cref="Current"/>.
        /// The <see cref="DetectedChanges"/> compares <see cref="Current"/> against <see cref="Source"/>
        /// </summary>
        public T? Source
        {
            get => _source;
            private set
            {
                _source = value;
                DetectChanges();
            }
        }

        /// <summary>
        /// Set to a valid instance of <see cref="Exception"/> if the latest attempt to read or validate date was unsuccessful.
        /// </summary>
        public Exception? LastException { get; private set; }

        /// <summary>
        /// Starts the asynchronous monitoring task. Should be called only once per given <paramref name="cancellationToken"/>.
        /// Cancel the existing <see cref="CancellationTokenSource"/> to being able to call this function again with a new token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The Task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">When the task was already started before.</exception>
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
        /// Enforce the source data reading and progress report sequence.
        /// </summary>
        /// <returns>True on success.</returns>
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

        /// <summary>
        /// Clones the <see cref="Source"/> to become the new <see cref="Current"/>.
        /// This is so that the caller can make the final decision despite changes were detected.
        /// <seealso cref="DetectedChanges"/>
        /// </summary>
        public void PromoteSourceAsCurrent() => Current = (T?)Source?.Clone();

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

        private void ThrowIfAlreadyStarted()
        {
            if (_started)
                throw new InvalidOperationException($"{nameof(StartMonitoringAsync)} was already invoked before.");

            _started = true;
        }
    }
}
