using System;
using System.Timers;

namespace Testy.Timers;

/// <summary>
/// Periodically invokes the passed-in callback
/// </summary>
public class PeriodicCallback : IDisposable
{
    readonly Timer _timer = new();

    /// <summary>
    /// Creates the periodic callback and starts it
    /// </summary>
    public PeriodicCallback(TimeSpan interval, Action callback)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        _timer.Interval = interval.TotalMilliseconds;
        _timer.Elapsed += (_, _) =>
        {
            try
            {
                callback();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error in periodic callback: {exception}");
            }
        };
        _timer.Start();
    }

    /// <summary>
    /// Stops the periodic callback
    /// </summary>
    public void Dispose() => _timer.Dispose();
}