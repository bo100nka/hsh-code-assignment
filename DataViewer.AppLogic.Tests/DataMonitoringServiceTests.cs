using DataViewer.Interfaces;
using DataViewer.Models.Data;
using Moq;

namespace DataViewer.AppLogic.Tests
{
    public sealed class DataMonitoringServiceTests
    {
        [Fact]
        public void ctor_ValidatesArguments()
        {
            // Arrange
            IDataParser<BooksLibrary> validParser = new Mock<IDataParser<BooksLibrary>>().Object;
            IDataValidator<BooksLibrary> validValidator = new Mock<IDataValidator<BooksLibrary>>().Object;

            // Act, Assert
            ArgumentNullException ex1 = Assert.Throws<ArgumentNullException>(() => new DataMonitoringService<BooksLibrary>(dataParser: null!, validValidator, TimeSpan.FromSeconds(1)));
            ArgumentNullException ex2 = Assert.Throws<ArgumentNullException>(() => new DataMonitoringService<BooksLibrary>(validParser, dataValidator: null!, TimeSpan.FromSeconds(1)));
            ArgumentException ex3 = Assert.Throws<ArgumentException>(() => new DataMonitoringService<BooksLibrary>(validParser, validValidator, TimeSpan.Zero));

            Assert.Equal("dataParser", ex1.ParamName);
            Assert.Equal("dataValidator", ex2.ParamName);
            Assert.Equal("timerInterval", ex3.ParamName);

            using var unit = new DataMonitoringService<BooksLibrary>(validParser, validValidator, TimeSpan.FromSeconds(1));
            Assert.NotNull(unit);
        }

        [Fact]
        public async Task StartMonitoringAsync_CanOnlyBeCalledOnce()
        {
            // Arrange
            IDataParser<BooksLibrary> validParser = new Mock<IDataParser<BooksLibrary>>().Object;
            IDataValidator<BooksLibrary> validValidator = new Mock<IDataValidator<BooksLibrary>>().Object;
            using var unit = new DataMonitoringService<BooksLibrary>(validParser, validValidator, TimeSpan.FromMilliseconds(1));

            _ = unit.StartMonitoringAsync(CancellationToken.None);

            // Act, Assert
            InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => unit.StartMonitoringAsync(CancellationToken.None));

            Assert.Equal($"{nameof(DataMonitoringService<BooksLibrary>.StartMonitoringAsync)} was already invoked before.", ex.Message);
        }

        [Fact]
        public async Task MonitoringService_ShouldDetectBookChanges()
        {
            // TODO: FYI Because i have separated parsing logic from the monitoring service, i do not simulate JSON file here,
            // instead i mock the behavior of the injected parser service and simulate changes on the valid instance instead
            // I hope that it is still OK.

            // TODO: FYI I have decided to give the monitoring service much more interaction logic (API) to be able to
            // notify the caller about details of exceptions and steps, hence the test is a bit deeper
            // i hope it's OK.

            // Arrange
            BooksLibrary booksLibrarySource = GetValidBooksLibrary();
            BooksLibrary expectedCurrent = GetValidBooksLibrary();
            var parserMock = new Mock<IDataParser<BooksLibrary>>();
            parserMock
                .Setup(m => m.Parse())
                .Returns(booksLibrarySource);
            var validatorMock = new Mock<IDataValidator<BooksLibrary>>();
            validatorMock.Setup(m => m.Validate(It.IsAny<BooksLibrary>())); // dont care about validation, only that it has been called
            using var service = new DataMonitoringService<BooksLibrary>(parserMock.Object, validatorMock.Object, TimeSpan.FromMilliseconds(1)); // Fast interval for testing
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken interruptToken = cancellationTokenSource.Token;
            const int waitTimeInMilliseconds = 20;

            int progressReportCount = 0;
            void service_OnProgress(object? sender, EventArgs? eventArgs) => progressReportCount++;
            service.OnProgress += service_OnProgress;

            // Assert just to expect default values of relevant public properties before we run the main async monitoring loop
            Assert.Null(service.LastException);
            Assert.Null(service.Source);
            Assert.Null(service.Current);
            Assert.False(service.DetectedChanges);
            Assert.False(service.LastReloadSucceeded);

            // Act: let the main loop run asynchronously in background
            Task monitoringTask = service.StartMonitoringAsync(interruptToken);

            await Task.Delay(waitTimeInMilliseconds, CancellationToken.None); // 1ms time interval above should be covered by 10ms wait

            // Assert: first time parsed data came in but it was still not "accepted" by the client (using UpdateCurrent())
            Assert.Null(service.LastException);
            Assert.NotNull(service.Source);
            Assert.Null(service.Current);
            Assert.True(service.DetectedChanges);
            Assert.True(service.LastReloadSucceeded);

            // Act: Promote the latest Source data as Current
            service.PromoteSourceAsCurrent();

            // Assert: Current should be equal to Source and DetectedChanges indicator should be false
            Assert.Null(service.LastException);
            Assert.NotNull(service.Source);
            Assert.NotNull(service.Current);
            Assert.Equal(expectedCurrent, service.Current);
            Assert.False(service.DetectedChanges);
            Assert.True(service.LastReloadSucceeded);

            await Task.Delay(waitTimeInMilliseconds, CancellationToken.None);

            // Assert: Initial state (books are equal, no change)
            Assert.False(service.DetectedChanges, $"{nameof(service.DetectedChanges)} should be false when books are equal.");

            // Act: Modify Source book library
            booksLibrarySource.Version = "9.8.7";
            booksLibrarySource.Articles![0]!.Title = "Modified book";

            // Wait for next tick to detect change
            await Task.Delay(waitTimeInMilliseconds, CancellationToken.None);

            // Assert: Change detected
            Assert.True(service.DetectedChanges, $"{nameof(service.DetectedChanges)} should be true after Source book is modified.");
            Assert.NotNull(service.Current);
            Assert.NotNull(service.Source);
            Assert.NotEqual(service.Current, service.Source);
            Assert.True(progressReportCount > 0);

            // Act: verify that it's cancellable
            cancellationTokenSource.Cancel();

            // Assert this should not throw TaskCancelledException or OperationCancelledException
            await monitoringTask;

            // Cleanup
            service.OnProgress -= service_OnProgress;
        }

        private static BooksLibrary GetValidBooksLibrary() => new()
        {
            Version = "1.0",
            Timestamp = "2025-04-18 17:48",
            Articles =
            [
                new BooksLibraryArticle
                {
                    Title = "Test Book",
                    Author = "Author",
                    Isbn13 = "123",
                },
                new BooksLibraryArticle
                {
                    Title = "Test Book 2",
                    Author = "Author 2",
                    Isbn13 = "234",
                }
            ]
        };
    }
}
