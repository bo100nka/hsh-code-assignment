using DataViewer.Interfaces;
using DataViewer.Models.Exceptions;

namespace DataViewer.AppLogic
{
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

        public DataMonitoringService(IDataParser<T> dataParser, IDataValidator<T> dataValidator, TimeSpan timerInterval)
        {
            if (timerInterval == TimeSpan.Zero)
                throw new ArgumentException("Expected a positive time interval value.", nameof(timerInterval));
            _dataParser = dataParser ?? throw new ArgumentNullException(nameof(dataParser));
            _dataValidator = dataValidator ?? throw new ArgumentNullException(nameof(dataValidator));
            _timer = new PeriodicTimer(timerInterval);
            _disposed = false;
        }

        public event EventHandler? OnProgress;

        public bool DetectedChanges { get; private set; }

        public bool LastReloadSucceeded { get; private set; }

        public T? Current
        {
            get => _current;
            private set
            {
                _current = value;
                DetectChanges();
            }
        }

        public T? Source
        {
            get => _source;
            private set
            {
                _source = value;
                DetectChanges();
            }
        }

        public Exception? LastException { get; private set; }

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

        private void SetSucceeded()
        {
            LastException = null;
            LastReloadSucceeded = true;
        }

        public void PromoteSourceAsCurrent() => Current = (T?)Source?.Clone();

        public void Dispose()
        {
            if (_disposed)
                return;

            _timer.Dispose();
            _disposed = true;
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
