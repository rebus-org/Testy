using System;
// ReSharper disable UnusedMember.Global

namespace Testy.General
{
    /// <summary>
    /// Action that gets called when the object is disposed
    /// </summary>
    public class DisposableCallback : IDisposable
    {
        readonly Action _action;

        /// <summary>
        /// Creates the disposable callback with the given <paramref name="action"/>
        /// </summary>
        public DisposableCallback(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        /// <summary>
        /// Invokes the callback
        /// </summary>
        public void Dispose() => _action();
    }
}