﻿using System;
using System.ComponentModel;

namespace WindowsDesktop
{
    public partial class VirtualDesktop
    {
        /// <summary>
        /// Occurs when a virtual desktop is created.
        /// </summary>
        public static event EventHandler<VirtualDesktop> Created;

        public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyBegin;

        public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyFailed;

        /// <summary>
        /// Occurs when a virtual desktop is destroyed.
        /// </summary>
        public static event EventHandler<VirtualDesktopDestroyEventArgs> Destroyed;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler ApplicationViewChanged;

        /// <summary>
        /// Occurs when the current virtual desktop is changed.
        /// </summary>
        public static event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;

        /// <summary>
        /// Occurs when a virtual desktop is moved.
        /// </summary>
        public static event EventHandler<VirtualDesktopMovedEventArgs> Moved;

        /// <summary>
        /// Occurs when a virtual desktop is renamed.
        /// </summary>
        public static event EventHandler<VirtualDesktopRenamedEventArgs> Renamed;

        /// <summary>
        /// Occurs when the wallpaper in the virtual desktop is changed.
        /// </summary>
        public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs> WallpaperChanged;

        public static event EventHandler<VirtualDesktopSwitchedEventArgs> Switched;

        public static event EventHandler<VirtualDesktopRemoteConnectedEventArgs> RemoteConnected;

        internal static class EventRaiser
        {
            public static void RaiseCreated(object sender, VirtualDesktop pDesktop)
            {
                Created?.Invoke(sender, pDesktop);
            }

            public static void RaiseDestroyBegin(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
            {
                VirtualDesktopDestroyEventArgs args = new(pDesktopDestroyed, pDesktopFallback);
                DestroyBegin?.Invoke(sender, args);
            }

            public static void RaiseDestroyFailed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
            {
                VirtualDesktopDestroyEventArgs args = new(pDesktopDestroyed, pDesktopFallback);
                DestroyFailed?.Invoke(sender, args);
            }

            public static void RaiseDestroyed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
            {
                VirtualDesktopDestroyEventArgs args = new(pDesktopDestroyed, pDesktopFallback);
                Destroyed?.Invoke(sender, args);
            }

#pragma warning disable IDE0060
            public static void RaiseApplicationViewChanged(object sender, object pView)
            {
                ApplicationViewChanged?.Invoke(sender, EventArgs.Empty);
            }
#pragma warning restore IDE0060

            public static void RaiseCurrentChanged(object sender, VirtualDesktop pDesktopOld, VirtualDesktop pDesktopNew)
            {
                VirtualDesktopChangedEventArgs args = new(pDesktopOld, pDesktopNew);
                CurrentChanged?.Invoke(sender, args);
            }

            public static void RaiseMoved(object sender, VirtualDesktop pDesktopMoved, int oldIndex, int newIndex)
            {
                VirtualDesktopMovedEventArgs args = new(pDesktopMoved, oldIndex, newIndex);
                Moved?.Invoke(sender, args);
            }

            public static void RaiseRenamed(object sender, VirtualDesktop pDesktop, string name)
            {
                string oldName = pDesktop.Name;
                pDesktop.SetNameToCache(name);

                VirtualDesktopRenamedEventArgs args = new(pDesktop, oldName, name);
                Renamed?.Invoke(sender, args);
            }

            public static void RaiseWallpaperChanged(object sender, VirtualDesktop pDesktop, string path)
            {
                string oldPath = pDesktop.WallpaperPath;
                pDesktop.SetDesktopWallpaperToCache(path);

                VirtualDesktopWallpaperChangedEventArgs args = new(pDesktop, oldPath, path);
                WallpaperChanged?.Invoke(sender, args);
            }

            public static void RaiseSwitched(object sender, VirtualDesktop pDesktop)
            {
                VirtualDesktopSwitchedEventArgs args = new(pDesktop);
                Switched?.Invoke(sender, args);
            }

            public static void RaiseRemoteConnected(object sender, VirtualDesktop pDesktop)
            {
                VirtualDesktopRemoteConnectedEventArgs args = new(pDesktop);
                RemoteConnected?.Invoke(sender, args);
            }
        }
    }
}
