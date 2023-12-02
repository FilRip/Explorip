using System;

namespace WindowsDesktop.Internal
{
    public class Disposable
    {
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }

        private sealed class AnonymousDisposable : IDisposable
        {
            private bool _isDisposed;
            private readonly Action _dispose;

            public AnonymousDisposable(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                _dispose();
            }
        }
    }
}
