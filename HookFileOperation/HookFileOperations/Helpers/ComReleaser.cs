﻿using System;
using System.Runtime.InteropServices;

namespace Explorip.HookFileOperations.Helpers;

public class ComReleaser<T> : IDisposable where T : class
{
    private T _obj;
    private bool disposedValue;

    public ComReleaser(T obj)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        if (!obj.GetType().IsCOMObject) throw new ArgumentOutOfRangeException("obj");
        _obj = obj;
    }

    public T Item { get { return _obj; } }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing && _obj != null)
            {
                Marshal.FinalReleaseComObject(_obj);
                _obj = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
