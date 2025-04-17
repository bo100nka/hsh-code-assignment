namespace DataViewer.AppLogic
{
    /// <summary>
    /// A primitive encapsulation of <see cref="PeriodicTimer"/>, which allows to be started manually,
    /// but can only be stopped by canceling the provided <see cref="CancellationToken"/>.
    /// </summary>
    /// <remarks>This could have been implemented better, using an interface so the 'delay' and loop logic
    /// can be mocked in unit tests more easily than it is now.</remarks>
    public sealed class PeriodicInvoker : IDisposable
    {
        private readonly PeriodicTimer _timer;
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Constructs a new instance of the periodic invoker.
        /// </summary>
        /// <param name="period">How often the underlying action should be invoked. Must be greater than <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="cancellationToken">A valid <see cref="CancellationToken"/> is required.</param>
        /// <exception cref="ArgumentException">If either parameters are invalid.</exception>
        public PeriodicInvoker(TimeSpan period, CancellationToken cancellationToken)
        {
            if (period == TimeSpan.Zero)
                throw new ArgumentException("A time span larger than zero is expected.", nameof(period));

            if (cancellationToken == CancellationToken.None)
                throw new ArgumentException($"A valid {nameof(cancellationToken)} is required.", nameof(cancellationToken));

            _timer = new PeriodicTimer(period);
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Fires and forgets the underlying `Wait and invoke` async task.
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException">When the <paramref name="action"/> is null.</exception>
        public void Start(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _ = LoopAsync(action);
        }

        /// <summary>
        /// The <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() => _timer?.Dispose();

        private async Task LoopAsync(Action action)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cancellationToken).ConfigureAwait(false))
                    action();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
