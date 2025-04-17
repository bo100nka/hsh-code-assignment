namespace DataViewer.AppLogic.Tests
{
    public sealed class PeriodicInvokerTests
    {
        [Fact]
        public void ctor_ValidatesArguments()
        {
            var ex1 = Assert.Throws<ArgumentException>(() => new PeriodicInvoker(TimeSpan.Zero, new CancellationTokenSource().Token));
            var ex2 = Assert.Throws<ArgumentException>(() => new PeriodicInvoker(TimeSpan.FromSeconds(0), new CancellationTokenSource().Token));
            var ex3 = Assert.Throws<ArgumentException>(() => new PeriodicInvoker(TimeSpan.FromSeconds(1), CancellationToken.None));

            Assert.Equal("period", ex1.ParamName);
            Assert.Equal("period", ex2.ParamName);
            Assert.Equal("cancellationToken", ex3.ParamName);
        }

        [Fact]
        public void Start_ValidatesArguments()
        {
            using var unit = new PeriodicInvoker(TimeSpan.FromMilliseconds(100), new CancellationTokenSource().Token);

            var ex = Assert.Throws<ArgumentNullException>(() => unit.Start(action: default!));

            Assert.Equal("action", ex.ParamName);
        }

        [Fact]
        public async Task Start_InvokesDelayedActionAndStopsUponCancelledToken()
        {
            // Arrange
            var data = 1;
            using var cts = new CancellationTokenSource();
            using var unit = new PeriodicInvoker(TimeSpan.FromMilliseconds(10), cts.Token);
            Action action = () =>
            {
                data++;

                if (data == 3)
                    cts.Cancel();
            };

            // Act
            unit.Start(action);

            // Assert
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            Assert.Equal(3, data);
        }
    }
}
