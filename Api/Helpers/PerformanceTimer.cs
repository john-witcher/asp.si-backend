using System.Diagnostics;

namespace Api.Helpers;

public class PerformanceTimer
{
    public static void TimeMethod(Action  methodToTime)
    {
            Console.WriteLine($"Timer started");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // Start timing
            methodToTime();    // Execute the method
            stopwatch.Stop();  // Stop timing
            
            Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds} ms");
        }
}