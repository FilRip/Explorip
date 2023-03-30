using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Explorip.HookFileOperations.Helpers
{
    public static class ExtensionsComInterface
    {
        public static void RechercheComInterface(IntPtr iUnknown, out Dictionary<Guid, string> guid)
        {
            guid = new Dictionary<Guid, string>();
            RegistryKey racineInterface = Registry.ClassesRoot.OpenSubKey("Interface");
            foreach (string @interface in racineInterface.GetSubKeyNames())
            {
                string nomInterface = racineInterface.OpenSubKey(@interface).GetValue("")?.ToString() ?? "Inconnu";
                Guid myGuid = new(@interface);
                try
                {
                    if (Marshal.QueryInterface(iUnknown, ref myGuid, out _) == 0)
                    {
                        guid.Add(myGuid, nomInterface);
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
