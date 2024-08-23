using Microsoft.Extensions.Internal;
using SubsituteDependencies;

namespace SubstituteDependenciesTests
{
    public class TestSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow { get; set; }

        [Fact]
        public void RestartStopwatch()
        {
            var clock = new TestSystemClock()
            {
                UtcNow = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero) 
            };
            var delay = TimeSpan.FromHours(2);
  
            var stopwatch = new BasicStopwatch(clock);
            stopwatch.Start();
            clock.UtcNow += delay;
            stopwatch.Stop();

            // Start should reset
            stopwatch.Start();
            clock.UtcNow += delay;
            stopwatch.Stop();

            Assert.NotNull(stopwatch.ElapsedTime);
            Assert.Equal(delay, stopwatch.ElapsedTime);
        }
    }
}