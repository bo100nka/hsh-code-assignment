using System.Collections.ObjectModel;
using System.Windows.Input;
using DataViewer.AppLogic;
using DataViewer.Common;
using DataViewer.Interfaces;
using DataViewer.Models;
using DataViewer.Models.Data;

namespace DataViewer.ViewModel
{
    /// <summary>
    /// The behavior logic for the corresponding view (only see <see cref="View.BooksLibraryView"/> is implemented though.)
    /// </summary>
    public sealed class BooksLibraryViewModel : ViewModelBase
    {
        private readonly IDataParser<BooksLibrary> _booksLibraryParser;
        private readonly IDataValidator<BooksLibrary> _booksLibraryValidator;
        private readonly PeriodicInvoker _periodicDataReloader;
        private string? _statusText;
        private BooksLibraryModel? _header;
        private ObservableCollection<BooksLibraryArticleModel?>? _articles;

        /// <summary>
        /// Constructs an instance of the view model with a data parser, validator and periodic data update handlers.
        /// </summary>
        /// <param name="parser">A provider of the <see cref="BooksLibrary"/> structure.</param>
        /// <param name="validator">A validator of a <see cref="BooksLibrary"/> instance.</param>
        /// <param name="periodicInvoker">A simple implementation of <see cref="PeriodicTimer"/>.</param>
        /// <remarks>Do not confuse <see cref="BooksLibrary"/> with <see cref="BooksLibraryModel"/></remarks>
        /// <exception cref="ArgumentNullException">When either <paramref name="parser"/>, <paramref name="validator"/> or <paramref name="periodicInvoker"/> is null.</exception>
        public BooksLibraryViewModel(
            IDataParser<BooksLibrary> parser,
            IDataValidator<BooksLibrary> validator,
            PeriodicInvoker periodicInvoker)
        {
            _booksLibraryParser = parser ?? throw new ArgumentNullException(nameof(parser));
            _booksLibraryValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _periodicDataReloader = periodicInvoker ?? throw new ArgumentNullException(nameof(periodicInvoker));
            CommandForceReload = new RelayCommand(_ => TryReloadData());

            CommandForceReload.Execute(default);
            _periodicDataReloader.Start(() => CommandForceReload.Execute(default));
        }

        /// <summary>
        /// Manual force data reload command binding.
        /// </summary>
        public ICommand CommandForceReload { get; }

        /// <summary>
        /// A primitive status reporting text binding
        /// </summary>
        public string? StatusText
        {
            get => _statusText;
            set => SetAndNotifyIfNewValue(ref _statusText, value, nameof(StatusText));
        }

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
            base.Dispose();
            // disposing of injected classes is not done here, as whoever injected them to
            // this class should do it instead
        }

        private void TryReloadData()
        {
            var booksLibrary = default(BooksLibrary?);

            if (!Try("Loading data", () => booksLibrary = _booksLibraryParser.Parse()))
                return;

            if (!Try("Validating data", () => _booksLibraryValidator.Validate(booksLibrary!)))
                return;

            if (Try("Rebuilding model", () => RebuildBoundDataFrom(booksLibrary)))
                Report("Data loaded successfully");
        }

        /// <summary>
        /// a better solution is to simply have a main model as a single unit
        /// where each bound property gets updated on their own, i just
        /// went down this road instead - hard reload on each "refresh" sequence
        /// as is the nature of the assignment.
        /// </summary>
        /// <param name="booksLibrary"></param>
        private void RebuildBoundDataFrom(BooksLibrary? booksLibrary)
        {
            Header = new BooksLibraryModel(booksLibrary!);

            if (booksLibrary?.Articles?.Any() ?? false)
            {
                Articles = new ObservableCollection<BooksLibraryArticleModel?>();

                foreach (var article in booksLibrary.Articles)
                    Articles.Add(new BooksLibraryArticleModel(article));
            }
        }

        // and some helper functions below just for redability 
        private void Report(string status) => StatusText = $"[{DateTime.Now:HH:mm:ss}]: {status}...";

        private void Report(Exception exception) => StatusText = $"{StatusText} Error: {exception.FormatException(includeStackTrace: false)}";

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
