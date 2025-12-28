using System;

namespace VirtualDesktop.Utils;

internal class Disposable
{
    public static IDisposable Create(Action dispose)
    {
        return new AnonymousDisposable(dispose);
    }

    private sealed class AnonymousDisposable(Action dispose) : IDisposable
    {
        private bool _isDisposed;
        private readonly Action _dispose = dispose;

        public void Dispose()
        {
            if (this._isDisposed)
                return;

            this._isDisposed = true;
            this._dispose();
        }
    }
}
