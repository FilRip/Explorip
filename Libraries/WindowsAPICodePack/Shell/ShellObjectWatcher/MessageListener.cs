﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.Interop.ShellObjectWatcher;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.ShellObjectWatcher;

internal class MessageListener : IDisposable
{
    public const uint CreateWindowMessage = (uint)WindowMessage.User + 1;
    public const uint DestroyWindowMessage = (uint)WindowMessage.User + 2;
    public const uint BaseUserMessage = (uint)WindowMessage.User + 5;

    private const string MessageWindowClassName = "MessageListenerClass";

    private static readonly object _threadlock = new();
    private static uint _atom;
    private static Thread _windowThread = null;
    private static volatile bool _running = false;

    private static readonly ShellObjectWatcherNativeMethods.WndProcDelegate wndProc = WndProc;
    // Dictionary relating window's hwnd to its message window
    private static readonly Dictionary<IntPtr, MessageListener> _listeners = [];
    private static IntPtr _firstWindowHandle = IntPtr.Zero;

    private static readonly object _crossThreadWindowLock = new();
    private static IntPtr _tempHandle = IntPtr.Zero;

#pragma warning disable S3264 // Events should be invoked
    public event EventHandler<WindowMessageEventArgs> MessageReceived;
#pragma warning restore S3264 // Events should be invoked

    private static void SetFirstWindowHandle(IntPtr newValue)
    {
        _firstWindowHandle = newValue;
    }
    private static void SetWindowThread(Thread newThread)
    {
        _windowThread = newThread;
    }
    public MessageListener()
    {
        lock (_threadlock)
        {
            if (_windowThread == null)
            {
                SetWindowThread(new Thread(ThreadMethod));
                _windowThread.SetApartmentState(ApartmentState.STA);
                _windowThread.Name = "ShellObjectWatcherMessageListenerHelperThread";

                lock (_crossThreadWindowLock)
                {
                    _windowThread.Start();
                    Monitor.Wait(_crossThreadWindowLock);
                }

                SetFirstWindowHandle(WindowHandle);
            }
            else
            {
                CrossThreadCreateWindow();
            }

            if (WindowHandle == IntPtr.Zero)
            {
                throw new ShellException(LocalizedMessages.MessageListenerCannotCreateWindow,
                    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            }

            _listeners.Add(WindowHandle, this);
        }
    }

    private void CrossThreadCreateWindow()
    {
        if (_firstWindowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException(LocalizedMessages.MessageListenerNoWindowHandle);
        }

        lock (_crossThreadWindowLock)
        {
            CoreNativeMethods.PostMessage(_firstWindowHandle, (WindowMessage)CreateWindowMessage, IntPtr.Zero, IntPtr.Zero);
            Monitor.Wait(_crossThreadWindowLock);
        }

        WindowHandle = _tempHandle;
    }

    private static void RegisterWindowClass()
    {
        WindowClassEx classEx = new()
        {
            ClassName = MessageWindowClassName,
            WndProc = wndProc,

            Size = (uint)Marshal.SizeOf(typeof(WindowClassEx))
        };

        uint atom = ShellObjectWatcherNativeMethods.RegisterClassEx(ref classEx);
        if (atom == 0)
        {
            throw new ShellException(LocalizedMessages.MessageListenerClassNotRegistered,
                Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        }
        _atom = atom;
    }

    private static IntPtr CreateWindow()
    {
        IntPtr handle = ShellObjectWatcherNativeMethods.CreateWindowEx(
            0, //extended style
            MessageWindowClassName, //class name
            "MessageListenerWindow", //title
            0, //style
            0, 0, 0, 0, // x,y,width,height
            new IntPtr(-3), // -3 = Message-Only window
            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        return handle;
    }

    private static void SetRunning(bool newValue)
    {
        _running = newValue;
    }
    private void ThreadMethod() // Message Loop
    {
        lock (_crossThreadWindowLock)
        {
            SetRunning(true);
            if (_atom == 0)
            {
                RegisterWindowClass();
            }
            WindowHandle = CreateWindow();

            Monitor.Pulse(_crossThreadWindowLock);
        }

        while (_running)
        {
            if (ShellObjectWatcherNativeMethods.GetMessage(out Message msg, IntPtr.Zero, 0, 0))
            {
                ShellObjectWatcherNativeMethods.DispatchMessage(ref msg);
            }
        }
    }

    private static int WndProc(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam)
    {
        switch (msg)
        {
            case CreateWindowMessage:
                lock (_crossThreadWindowLock)
                {
                    _tempHandle = CreateWindow();
                    Monitor.Pulse(_crossThreadWindowLock);
                }
                break;
            case (uint)WindowMessage.Destroy:
                break;
            default:
                if (_listeners.TryGetValue(hwnd, out MessageListener listener))
                {
                    Message message = new(hwnd, msg, wparam, lparam, 0, new NativePoint());
                    listener.MessageReceived?.SafeRaise(listener, new WindowMessageEventArgs(message));
                }
                break;
        }

        return ShellObjectWatcherNativeMethods.DefWindowProc(hwnd, msg, wparam, lparam);
    }

    public IntPtr WindowHandle { get; private set; }
    public static bool Running { get { return _running; } }

    #region IDisposable Members

    ~MessageListener()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (_threadlock)
            {
                _listeners.Remove(WindowHandle);
                if (_listeners.Count == 0)
                {
                    CoreNativeMethods.PostMessage(WindowHandle, WindowMessage.Destroy, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }
    }

    #endregion
}


/// <summary>
/// Encapsulates the data about a window message 
/// </summary>
public class WindowMessageEventArgs : EventArgs
{
    /// <summary>
    /// Received windows message.
    /// </summary>
    public Message Message { get; private set; }

    internal WindowMessageEventArgs(Message msg)
    {
        Message = msg;
    }
}
