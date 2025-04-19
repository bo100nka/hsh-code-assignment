using System.Collections.ObjectModel;
using System.Windows.Input;
using DataViewer.AppLogic;
using DataViewer.Common;
using DataViewer.Models;
using DataViewer.Models.Data;

namespace DataViewer.ViewModel
{
    /// <summary>
    /// The behavior logic for the corresponding view (only see <see cref="View.BooksLibraryView"/> is implemented though.)
    /// </summary>
    public sealed class BooksLibraryViewModel : ViewModelBase
    {
        private CancellationTokenSource? _stoppingTokenSource;
        private readonly DataMonitoringService<BooksLibrary> _booksLibraryMonitoring;
        private Task? _booksLibraryMonitoringTask;
        private string? _statusText;
        private BooksLibraryModel? _header;
        private ObservableCollection<BooksLibraryArticleModel?>? _articles;

        /// <summary>
        /// Constructs an instance of the view model with a data parser, validator and periodic data update handlers.
        /// </summary>
        /// <param name="monitoringService">A periodic data monitoring.</param>
        /// <remarks>Do not confuse <see cref="BooksLibrary"/> with <see cref="BooksLibraryModel"/></remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="monitoringService"/> is null.</exception>
        public BooksLibraryViewModel(DataMonitoringService<BooksLibrary> monitoringService)
        {
            _booksLibraryMonitoring = monitoringService ?? throw new ArgumentNullException(nameof(monitoringService));
            _booksLibraryMonitoring.OnProgress += BooksLibraryMonitoring_OnProgress;

            CommandForceReload = new RelayCommand(ForceReloadData, IsRunning);
            CommandCancel = new RelayCommand(TryCancel, IsRunning);
            CommandRestart = new RelayCommand(RestartMonitoring, IsNotRunning);

            RestartMonitoring(null);
        }

        private void BooksLibraryMonitoring_OnProgress(object? sender, EventArgs e)
        {
            if (_booksLibraryMonitoring.LastReloadSucceeded)
            {
                if (_booksLibraryMonitoring.DetectedChanges || _hadError)
                    ReloadData();
            }
            else if (_booksLibraryMonitoring.LastException != null)
            {
                Report(_booksLibraryMonitoring.LastException);
            }
        }

        public bool IsNotRunning(object? parameter) => !IsRunning(parameter);

        public bool IsRunning(object? parameter) => _stoppingTokenSource != null;

        /// <summary>
        /// Manual force data reload command binding.
        /// </summary>
        public ICommand CommandForceReload { get; }

        public ICommand CommandCancel { get; }

        public ICommand CommandRestart { get; }

        /// <summary>
        /// A primitive status reporting text binding
        /// </summary>
        public string? StatusText
        {
            get => _statusText;
            set => SetAndNotifyIfNewValue(ref _statusText, value, nameof(StatusText));
        }

        private bool _hadError;

        /// <summary>
        /// A "header" part of the bound <see cref="BooksLibrary"/>.
        /// </summary>
        public BooksLibraryModel? Header
        {
            get => _header;
            set => SetAndNotifyIfNewValue(ref _header, value, nameof(Header));
        }

        /// <summary>
        /// A list of articles inside <see cref="BooksLibrary.Articles"/> property binding.
        /// </summary>
        public ObservableCollection<BooksLibraryArticleModel?>? Articles
        {
            get => _articles;
            set => SetAndNotifyIfNewValue(ref _articles, value, nameof(Articles));
        }

        /// <summary>
        /// The <see cref="IDisposable"/> implementation.
        /// </summary>
        public override void Dispose()
        {
            TryCancel(null);
            _booksLibraryMonitoring.OnProgress -= BooksLibraryMonitoring_OnProgress;

            // disposing of injected classes is not done here, as whoever injected them to
            // this class should do it instead

            base.Dispose();

            // and since we don't use an explicit finalizer (destructor), i dont make use of GC.SuppressFinalize() anywhere
        }

        private void RestartMonitoring(object? parameter)
        {
            // task is awaited later in TryCancel
            _stoppingTokenSource = new CancellationTokenSource();

            Report("Starting monitoring...");
            _booksLibraryMonitoringTask = Task.Run(() => _booksLibraryMonitoring.StartMonitoringAsync(_stoppingTokenSource.Token));

            ForceReloadData(parameter);
        }

        private void TryCancel(object? parameter)
        {
            if (_stoppingTokenSource == null)
                return;

            _stoppingTokenSource?.Cancel();
            _stoppingTokenSource?.Dispose();
            _stoppingTokenSource = null;

            try
            {
                // as Dispose() is not an async function, i await it's result synchronously to ensure completion
                _booksLibraryMonitoringTask?.GetAwaiter().GetResult();

                // and since awaiting a task is internally cleaned up in .net automatically, we don't need to call .Dispose() on it
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
                Report("Cancelled.");
            }
        }

        private void ForceReloadData(object? parameter)
        {
            _booksLibraryMonitoring.MonitorSourceData();
            ReloadData(true);
        }

        private void ReloadData(bool force = false)
        {
            _booksLibraryMonitoring.PromoteSourceAsCurrent();
            BooksLibrary? booksLibrary = _booksLibraryMonitoring.Current;
            Exception? lastException = _booksLibraryMonitoring.LastException;

            UpdateView(booksLibrary, lastException, force);
        }

        private void UpdateView(BooksLibrary? booksLibrary, Exception? lastException, bool force)
        {
            if (Try("Rebuilding model", () => RebuildBoundDataFrom(booksLibrary)))
            {
                Report($"Data loaded {(force ? "by force" : "successfully")}");
            }

            else if (lastException != null)
            {
                Report(lastException);
            }
        }

        /// <summary>
        /// a better solution is to simply have a main model as a single unit
        /// where each bound property gets updated on their own, i just
        /// went down this road instead - hard reload on each "refresh" sequence
        /// </summary>
        /// <param name="booksLibrary"></param>
        private void RebuildBoundDataFrom(BooksLibrary? booksLibrary)
        {
            Header = new BooksLibraryModel(booksLibrary);

            if (booksLibrary?.Articles != null && booksLibrary.Articles.Length > 0)
            {
                Articles = [];

                foreach (BooksLibraryArticle? article in booksLibrary.Articles)
                    Articles.Add(new BooksLibraryArticleModel(article));
            }
        }

        // and some helper functions below just for redability 
        private void Report(string status)
        {
            StatusText = $"[{DateTime.Now:HH:mm:ss}]: {status}...";
            _hadError = false;
        }

        private void Report(Exception exception)
        {
            StatusText = $"Error: {exception.FormatException(includeStackTrace: false)}";
            _hadError = true;
        }

        private bool Try(string status, Action action)
        {
            try
            {
                Report(status);
                action();
                return true;
            }
            catch (Exception ex)
            {
                Report(ex);
                return false;
            }
        }
    }
}
