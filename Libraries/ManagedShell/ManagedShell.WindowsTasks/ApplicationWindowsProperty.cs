using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

using ManagedShell.Common.Helpers;

namespace ManagedShell.WindowsTasks;

public class ApplicationWindowsProperty(IntPtr handle) : INotifyPropertyChanged
{
    private DateTime? _dateStart = null;
    private uint? _procId;
    private string _winFileName;
    private int _position;
    private string _name;
    private IntPtr _handle = handle;
    private string _titre;
    private WindowState _state;
    private string _arguments;

    public string Name
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }

    }

    public IntPtr Handle
    {
        get { return _handle; }
        set
        {
            if (_handle != value)
            {
                _handle = value;
                OnPropertyChanged();
            }
        }
    }

    public string Title
    {
        get { return _titre; }
        set
        {
            if (_titre != value)
            {
                _titre = value;
                OnPropertyChanged();
            }
        }
    }

    public Guid VirtualDesktopId
    {
        get { return VirtualDesktop.VirtualDesktop.IsInitialized ? VirtualDesktop.VirtualDesktop.FromHwnd(Handle).Id : Guid.Empty; }
    }

    public WindowState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateTimeStart
    {
        get
        {
            if (!_dateStart.HasValue)
            {
                _dateStart = DateTime.MinValue;
                if (_procId.HasValue)
                {
                    Process process = Process.GetProcessById((int)_procId.Value);
                    if (process != null)
                        _dateStart = process.StartTime;
                }
            }

            return _dateStart.Value;
        }
    }

    public string WinFileName
    {
        get
        {
            if (string.IsNullOrEmpty(_winFileName))
                _winFileName = ShellHelper.GetPathForHandle(Handle);

            return _winFileName;
        }
        set
        {
            if (_winFileName != value)
                _winFileName = value;
        }
    }

    public string Arguments
    {
        get { return _arguments; }
        set
        {
            if (_arguments != value)
            {
                _arguments = value;
                OnPropertyChanged();
            }
        }
    }

    public uint? ProcId
    {
        get
        {
            if (!_procId.HasValue && Handle != IntPtr.Zero)
                _procId = ShellHelper.GetProcIdForHandle(Handle);
            return _procId;
        }
    }

    public int Position
    {
        get { return _position; }
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged();
            }
        }
    }

    #region INotifyPropertyChanged Members

#nullable enable

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName()] string? PropertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

#nullable disable

    #endregion
}
