using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.ShellFolders;

namespace ManagedShell.UWPInterop
{
    public sealed class StoreApp : IEquatable<StoreApp>
    {
        public readonly string AppUserModelId;
        public string DisplayName { get; set; }
        public string IconColor { get; set; }

        public string SmallIconPath { get; set; }
        public string MediumIconPath { get; set; }
        public string LargeIconPath { get; set; }
        public string ExtraLargeIconPath { get; set; }
        public string JumboIconPath { get; set; }

        public StoreApp(string appUserModelId)
        {
            AppUserModelId = appUserModelId;
        }

        private ImageSource GetShellItemImageSource(IconSize size)
        {
            ShellItem item = new("shell:appsfolder\\" + AppUserModelId)
            {
                AllowAsync = false
            };
            ImageSource img = size switch
            {
                IconSize.Small => item.SmallIcon,
                IconSize.ExtraLarge => item.ExtraLargeIcon,
                IconSize.Jumbo => item.JumboIcon,
                _ => item.LargeIcon,
            };
            item.Dispose();

            if (img != null)
            {
                return img;
            }

            return IconImageConverter.GetDefaultIcon();
        }

        public ImageSource GetIconImageSource(IconSize size)
        {
            string iconPath = size switch
            {
                IconSize.Small => SmallIconPath,
                IconSize.Medium => MediumIconPath,
                IconSize.ExtraLarge => ExtraLargeIconPath,
                IconSize.Jumbo => JumboIconPath,
                _ => LargeIconPath,
            };
            if (string.IsNullOrEmpty(iconPath))
            {
                return GetShellItemImageSource(size);
            }

            try
            {
                BitmapImage img = new();
                img.BeginInit();
                img.UriSource = new Uri(iconPath, UriKind.Absolute);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();

                return img;
            }
            catch (Exception e)
            {
                ShellLogger.Debug($"StoreApp: Unable to load icon by path for {DisplayName}: {e.Message}");
                return GetShellItemImageSource(size);
            }
        }

        #region IEquitable
        public bool Equals(StoreApp other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return AppUserModelId == other.AppUserModelId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StoreApp)obj);
        }

        public override int GetHashCode()
        {
            return AppUserModelId.GetHashCode();
        }

        #endregion
    }
}
