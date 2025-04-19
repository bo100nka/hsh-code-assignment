namespace DataViewer.Interfaces
{
    /// <summary>
    /// A data monitoring service that handles caching of existing and newly detected data
    /// and comparison between them along with progress reporting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataMonitoringService<T>
        where T : ICloneable, IEquatable<T>
    {
        /// <summary>
        /// Holds the latest Source data that the <see cref="Current"/> is compared against.
        /// Call the <see cref="PromoteSourceAsCurrent"/> to make the Source become the new <see cref="Current"/>.
        /// The <see cref="DetectedChanges"/> compares <see cref="Current"/> against <see cref="Source"/>
        /// </summary>
        T? Source { get; }

        /// <summary>
        /// Holds the Current data instance promoted from <see cref="Source"/>
        /// earlier by calling <see cref="PromoteSourceAsCurrent"/>.
        /// The <see cref="DetectedChanges"/> compares <see cref="Current"/> against <see cref="Source"/>
        /// </summary>
        T? Current { get; }

        /// <summary>
        /// Gets whether there are changes between <see cref="Current"/> and <see cref="Source"/>.
        /// </summary>
        bool DetectedChanges { get; }

        /// <summary>
        /// Set to a valid instance of <see cref="Exception"/> if the latest attempt to
        /// read or validate date was unsuccessful.
        /// </summary>
        Exception? LastException { get; }

        /// <summary>
        /// Gets whether the latest attempt to reload data was successful. <seealso cref="LastException"/>.
        /// </summary>
        bool LastReloadSucceeded { get; }

        /// <summary>
        /// Fired during various stages of the monitoring sequence.
        /// </summary>
        event EventHandler? OnProgress;

        /// <summary>
        /// Enforce the source data reading and progress report sequence.
        /// </summary>
        /// <returns>True on success.</returns>
        bool MonitorSourceData();

        /// <summary>
        /// Clones the <see cref="Source"/> to become the new <see cref="Current"/>.
        /// This is so that the caller can make the final decision despite changes were detected.
        /// <seealso cref="DetectedChanges"/>
        /// </summary>
        void PromoteSourceAsCurrent();

        /// <summary>
        /// Starts the asynchronous monitoring task. Should be called only once per given <paramref name="cancellationToken"/>.
        /// Cancel the existing <see cref="CancellationTokenSource"/> to being able to call this function again with a new token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The Task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">When the task was already started before.</exception>
        Task StartMonitoringAsync(CancellationToken cancellationToken);
    }
}
