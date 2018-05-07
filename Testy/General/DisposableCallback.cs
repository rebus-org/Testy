using System;

namespace Testy
{
    /// <summary>
    /// Action that gets called when the object is disposed
    /// </summary>
    public class DisposableCallback : IDisposable
    {
        readonly Action _action;

        public DisposableCallback(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Dispose() => _action();
    }
}