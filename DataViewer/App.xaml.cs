// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows;
using System.Windows.Threading;

namespace DataViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            object viewModel = null;
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
            MessageBox.Show(FormatException(e.Exception), "DispatcherUnhandledException", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static string FormatException(Exception exception)
        {
            return string.Format(
                "DispatcherUnhandledException: {0}: {1}\n\nStack trace:\n{2}",
                exception.GetType().FullName,
                exception.Message,
                exception.StackTrace);
        }
    }

}
