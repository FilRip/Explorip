using System.Reflection;
using System.Threading;

namespace Explorip.Helpers;

public static class ExtensionsTimer
{
    public static bool IsDisposed(this Timer timer)
    {
        if (timer == null)
            return true;
#pragma warning disable IDE0079
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        object timerHolder = timer.GetType().GetField("m_timer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(timer);
        object timerQueue = timerHolder.GetType().GetField("m_timer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(timerHolder);
        bool disposed = (bool)timerQueue.GetType().GetField("m_canceled", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(timerQueue);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning restore IDE0079
        return disposed;
    }
}
