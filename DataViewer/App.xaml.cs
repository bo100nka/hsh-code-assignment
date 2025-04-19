using System.IO;
using System.Windows;
using System.Windows.Threading;
using DataViewer.AppLogic;
using DataViewer.Common;
using DataViewer.Interfaces;
using DataViewer.Models.Data;
using DataViewer.ViewModel;

namespace DataViewer
{
    /// <summary>
    /// Considered the entry point of a WPF application, used this "backend" xaml.cs code
    /// as an exception in having a clear MVVM implementation as I wanted to create
    /// instances of the main view and it's (backend) view model and bind them together.
    /// </summary>
    public partial class App : Application, IDisposable
    {
        // although pointless as not used, but better than a magic constant somewhere down there
        public const int UnexpectedExitCode = -1;

        // went with a hardcoded file name
        public const string DataFileName = "books.json";

        // went with a hardcoded 2 seconds monitoring interval period
        public const int DataRefreshRateMilliseconds = 2000;

        private IDataParser<BooksLibrary>? _parser;
        private IDataValidator<BooksLibrary>? _validator;
        private DataMonitoringService<BooksLibrary>? _monitoring;
        private BooksLibraryViewModel? _viewModel;

        /// <summary>
        /// The entry point of a WPF application.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 'unexpected' generic error message box handling.
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // not using any dependency injection, but at least injecting the dependencies manually to the VM here
            _parser = new BooksLibraryJsonFileParser(Path.Combine(".", DataFileName));
            _validator = new BooksLibraryValidator();
            _monitoring = new DataMonitoringService<BooksLibrary>(_parser, _validator, TimeSpan.FromMilliseconds(DataRefreshRateMilliseconds));

            // MVVM bindings are done via DataContext, the main window contains the actual View instance in XAML
            _viewModel = new BooksLibraryViewModel(_monitoring);
            new MainWindow
            {
                DataContext = _viewModel
            }.Show();
        }

        /// <summary>
        /// On exit simply kills the processes but here i try to cancel the existing Tasks
        /// for any child classes to carry on with their disposal tasks before a graceful shutdown.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e);
            e.Handled = true;
        }

        private static void HandleException(DispatcherUnhandledExceptionEventArgs e)
        {
            // this may be replaced with something more 'View' agnostic so that if we replace
            // the XAML with Console as View we won't pop up any UI controls, but i didn't go down that deep rabbit hole in this assignment
            // There could have been something like "if (UseUI)" on top
            MessageBox.Show(
                $"Unexpected error occured. The application will now shut down.\nError details:\n{e.Exception.FormatException(includeStackTrace: true)}",
                "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);

            // die
            App.Current.Shutdown(UnexpectedExitCode);
        }

        public void Dispose()
        {
            _viewModel?.Dispose();
            _monitoring?.Dispose();
            (_parser as IDisposable)?.Dispose();
            (_validator as IDisposable)?.Dispose();
        }
    }

}
