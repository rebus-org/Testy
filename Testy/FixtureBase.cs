using System;
using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework;
using Testy.Benchmarking;
using Testy.Files;
using Testy.Timers;

// ReSharper disable UnusedMember.Global

namespace Testy
{
    /// <summary>
    /// Fixture base on which tests can be based
    /// </summary>
    public abstract class FixtureBase
    {
        readonly ConcurrentStack<IDisposable> _disposables = new ConcurrentStack<IDisposable>();

        /// <summary>
        /// Called by NUnit before each test. Will call the virtual <see cref="SetUp"/> method, which can be overridden in concrete test fixtures.
        /// </summary>
        [SetUp]
        public void InnerSetUp()
        {
            SetUp();
        }

        /// <summary>
        /// Override this to be invoked before each test
        /// </summary>
        protected virtual void SetUp()
        {
        }

        /// <summary>
        /// Called by NUnit after each test. Will call <see cref="CleanUpDisposables"/>.
        /// </summary>
        [TearDown]
        public void InnerTearDown()
        {
            CleanUpDisposables();
        }

        /// <summary>
        /// Registers the given <paramref name="disposable"/> in the disposables stack, to be
        /// disposed when <see cref="CleanUpDisposables"/> is called (if not before, then at
        /// least when NUnit calls the <see cref="InnerTearDown"/> method).
        /// </summary>
        protected TDisposable Using<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
        {
            _disposables.Push(disposable);
            return disposable;
        }

        /// <summary>
        /// Disposes all disposables registered using <see cref="Using{TDisposable}"/>.
        /// The disposables are disposed in reverse order.
        /// </summary>
        protected void CleanUpDisposables()
        {
            while (_disposables.TryPop(out var disposable))
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates a new timer scope with the given <paramref name="description"/>. If <paramref name="countForRateCalculation"/>
        /// is set, a rate will be calculated and printed in addition to the duration.
        /// </summary>
        protected IDisposable TimerScope(string description, int? countForRateCalculation = null) => new TimerScope(description, countForRateCalculation);

        /// <summary>
        /// Creates a new periodic callback with the given <paramref name="interval"/>
        /// </summary>
        protected IDisposable PeriodicCallback(TimeSpan interval, Action callback) => new PeriodicCallback(interval, callback);

        /// <summary>
        /// Creates a new temporary test directory, which will automatically be removed after the test has finished executing
        /// </summary>
        protected TemporaryTestDirectory NewTempDirectory() => Using(new TemporaryTestDirectory());

        /// <summary>
        /// Returns a new <see cref="CancellationToken"/> which will be cancelled after <paramref name="delay"/>
        /// </summary>
        protected CancellationToken CancelAfter(TimeSpan delay) => Using(new CancellationTokenSource(delay: delay)).Token;
    }
}