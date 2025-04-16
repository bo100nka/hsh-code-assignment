using System.IO;
using System.Windows;
using System.Windows.Threading;
using DataViewer.AppLogic;
using DataViewer.Common;
using DataViewer.ViewModel;

namespace DataViewer
{
    public partial class App : Application
    {
        private const int UnexpectedExitCode = -1;
        private const string DataFileName = "books.json";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            var parser = new BooksLibraryJsonFileParser(Path.Combine(".", DataFileName));
            var validator = new BooksLibraryValidator();
            var viewModel = new BooksLibraryViewModel(parser, validator);
            var mainWindow = new MainWindow { DataContext = viewModel };

            mainWindow.Show();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e);
            e.Handled = true;
        }

        private static void HandleException(DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Unexpected error occured. The application will now shut down.\nError details:\n{e.Exception.FormatException(includeStackTrace: true)}",
                "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
            App.Current.Shutdown(UnexpectedExitCode);
        }
    }

}
