using System.Collections.ObjectModel;
using System.Windows.Input;
using DataViewer.Common;
using DataViewer.Interfaces;
using DataViewer.Models.Data;
using DataViewer.Models.Exceptions;

namespace DataViewer.ViewModel
{
    public sealed class BooksLibraryViewModel : ViewModelBase
    {
        private readonly IDataParser<BooksLibrary> _dataParser;
        private readonly IDataValidator<BooksLibrary> _dataValidator;
        private readonly ICommand? _commandForceReload;
        private string? _statusText;
        private bool _hasError;
        private BooksLibraryModel? _header;
        private ObservableCollection<BooksLibraryArticleModel?>? _articles;

        public BooksLibraryViewModel(IDataParser<BooksLibrary> parser, IDataValidator<BooksLibrary> validator)
        {
            _dataParser = parser ?? throw new ArgumentNullException(nameof(parser));
            _dataValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _commandForceReload = new RelayCommand(_ => TryReloadData());

            TryReloadData();
        }

        private void TryReloadData()
        {
            _hasError = true;
            var booksLibrary = default(BooksLibrary?);

            try
            {
                Report("Loading data");
                booksLibrary = _dataParser.Parse();

            }
            catch (DataParseException pex)
            {
                Report(pex);
            }

            try
            {
                Report("Validating data");
                _dataValidator.Validate(booksLibrary!);
                _hasError = false;
            }
            catch (DataValidationException vex)
            {
                Report(vex);
            }

            if (_hasError)
                return;

            Report("Rebuilding model");
            RebuildModel(booksLibrary);
            Report("Done!");
        }

        private void RebuildModel(BooksLibrary? booksLibrary)
        {
            Header = new BooksLibraryModel(booksLibrary!);

            if (booksLibrary?.Articles?.Any() ?? false)
            {
                Articles = new ObservableCollection<BooksLibraryArticleModel?>();

                foreach (var article in booksLibrary.Articles)
                    Articles.Add(new BooksLibraryArticleModel(article));
            }
        }

        private void Report(string status) => StatusText = $"[{DateTime.Now:HH:mm:ss}]: {status}...";
        private void Report(Exception exception) => StatusText = $"{StatusText} Error: {exception.FormatException(includeStackTrace: false)}";

        public ICommand CommandForceReload => _commandForceReload!;

        public string? StatusText
        {
            get => _statusText;
            set => SetAndNotifyIfNewValue(ref _statusText, value, nameof(StatusText));
        }

        public BooksLibraryModel? Header
        {
            get => _header;
            set => SetAndNotifyIfNewValue(ref _header, value, nameof(Header));
        }

        public ObservableCollection<BooksLibraryArticleModel?>? Articles
        {
            get => _articles;
            set => SetAndNotifyIfNewValue(ref _articles, value, nameof(Articles));
        }
    }
}
