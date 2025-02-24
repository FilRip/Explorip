using System;

namespace WindowsDesktop.Internal
{
    public class Disposable
    {
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }

        private sealed class AnonymousDisposable(Action dispose) : IDisposable
        {
            private bool _isDisposed;

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                dispose();
            }
        }
    }
}
