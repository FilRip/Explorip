using System;

using ManagedShell.Common.Logging;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

public class SafeNotifyIconData
{
    public IntPtr Hwnd { get; set; }
    public uint ID { get; set; }
    public ShellNotifyIcons Flags { get; set; }
    public uint CallbackMessage { get; set; }
    public IntPtr IconHandle { get; set; }
    public string Tip { get; set; }
    public ENotifyIconStatus State { get; set; }
    public ENotifyIconStatus StateMask { get; set; }
    public string Info { get; set; }
    public uint Version { get; set; }  // used with NIM_SETVERSION, values 0, 3 and 4
    public string InfoTitle { get; set; }
    public ENotifyIconInfos InfoFlags { get; set; }
    public Guid GuidItem { get; set; }
    public uint BalloonIconHandle { get; set; }

    public SafeNotifyIconData() { }

    public SafeNotifyIconData(NotifyIconData nid)
    {
        Hwnd = (IntPtr)nid.hWnd;
        ID = nid.uID;
        Flags = nid.uFlags;
        CallbackMessage = nid.uCallbackMessage;

        try
        {
            IconHandle = (IntPtr)nid.hIcon;
        }
        catch (Exception e)
        {
            ShellLogger.Error($"SafeNotifyIconData: Unable to convert icon handle: {e.Message}");
        }

        Tip = nid.szTip;
        State = nid.dwState;
        StateMask = nid.dwStateMask;
        Info = nid.szInfo;
        Version = nid.uTimeoutOrVersion;
        InfoTitle = nid.szInfoTitle;
        InfoFlags = nid.dwInfoFlags;
        GuidItem = nid.guidItem;
        BalloonIconHandle = nid.hBalloonIcon;
    }
}
