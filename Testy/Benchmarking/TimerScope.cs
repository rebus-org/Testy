using System;
using System.Diagnostics;

namespace Testy.Benchmarking
{
    /// <summary>
    /// Measures time spent between instantiation and disposal. Can be used to perform easy low-ceremony benchmarking.
    /// </summary>
    public class TimerScope : IDisposable
    {
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        readonly int? _countForRateCalculation;
        readonly string _description;

        /// <summary>
        /// Creates the scope
        /// </summary>
        public TimerScope(string description, int? countForRateCalculation = null)
        {
            _description = description;
            _countForRateCalculation = countForRateCalculation;
        }

        /// <summary>
        /// Disposes the scope
        /// </summary>
        public void Dispose()
        {
            var elapsedMs = _stopwatch.Elapsed.TotalMilliseconds;

            if (_countForRateCalculation == null)
            {
                Console.WriteLine($"SCOPE '{_description}' completed in {elapsedMs} ms");
            }
            else
            {
                var millisecondsPerItem = elapsedMs / _countForRateCalculation.GetValueOrDefault(0);
                var itemsPerMillisecond = _countForRateCalculation.GetValueOrDefault(0) / elapsedMs;

                Console.WriteLine($"SCOPE '{_description}' completed in {elapsedMs} ms | {millisecondsPerItem} ms/item | {itemsPerMillisecond} items/ms");
            }
        }
    }
}