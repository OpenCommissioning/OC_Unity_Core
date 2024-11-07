using System.Diagnostics;
using System.Threading.Tasks;

namespace OC.Communication
{
    public static class StopwatchExtension
    {
        /// <summary>
        /// Async wait extension for <see cref="T:System.Diagnostics.Stopwatch"/>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task.</param>
        /// </summary>
        public static async Task WaitUntil(this Stopwatch stopwatch, int millisecondsDelay)
        {
            var delta = millisecondsDelay - (int) stopwatch.Elapsed.TotalMilliseconds;
            if (delta > 0) await Task.Delay(millisecondsDelay);
            
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}