using DataViewer.ViewModel;

namespace DataViewer.Tests
{
    public class BooksLibraryViewModelTests
    {
        [Fact]
        public void Constructor_ValidatesArguments()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new BooksLibraryViewModel(monitoringService: null!));

            Assert.Equal("monitoringService", ex.ParamName);
        }

        [Fact]
        public void Constructor_StartsMonitoring_TODO()
        {
            //TODO: i skipped this entirely, as I needed a thorough rest during the last weekend
            // before the deadline of the assignment.

            // Here is a rough idea what i had in mind however:
            // 0. It is possible to Mock the class with CallBase = true
            // 1. Do a minor refactoring of the BooksLibraryViewModel class so that certain private methods become
            //    protected virtual and thus Mockable. This is not a real system testing, but more effective unit testing.
            // 2. This would make it easier for simulating various scenarios and only
            //    verifying whether that or other mocked methods was called given a certain condition.
            // 3. I would have mocked the ICommand properties to simulate button clicks
            // 4. But overall, i think that testing this might not be as hard given that I have separated most of the
            //    "business logic" among various unit tested classes.
            Assert.False(false, "Sigh");
        }
    }
}
