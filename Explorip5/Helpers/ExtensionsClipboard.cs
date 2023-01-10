using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

namespace Explorip.Helpers
{
    public static class ExtensionsClipboard
    {
        public static void AjouterFichiersDossiers(List<FileSystemInfo> fichiersDossiers, bool deplace = false)
        {
            if ((fichiersDossiers == null) || (fichiersDossiers.Count == 0))
                return;

            DragDropEffects dropEffect = (deplace ? DragDropEffects.Move : DragDropEffects.Copy);

            StringCollection listeFichiersDossiers = new();
            listeFichiersDossiers.AddRange(fichiersDossiers.Select(x => x.FullName).ToArray());

            DataObject data = new();
            data.SetFileDropList(listeFichiersDossiers);
            data.SetData("Preferred Dropeffect", new MemoryStream(BitConverter.GetBytes((int)dropEffect)));
            Clipboard.SetDataObject(data);
        }

        public static bool LireFichiersDossiers(bool supprimeLaListe, out List<FileSystemInfo> fichiersDossiers, out bool deplace)
        {
            fichiersDossiers = new List<FileSystemInfo>();
            deplace = false;
            bool retour = true;

            try
            {
                DataObject sortie;
                sortie = (DataObject)Clipboard.GetDataObject();
                StringCollection listeFichiersDossiers = sortie.GetFileDropList();
                foreach (string item in listeFichiersDossiers)
                {
                    if (Directory.Exists(item))
                        fichiersDossiers.Add(new DirectoryInfo(item));
                    else
                        fichiersDossiers.Add(new FileInfo(item));
                }
                MemoryStream dropEffect = (MemoryStream)sortie.GetData("Preferred Dropeffect");
                byte[] buffer = new byte[dropEffect.Length];
                if (dropEffect.CanRead)
                {
                    dropEffect.Read(buffer, 0, (int)dropEffect.Length);
                }
                int result = BitConverter.ToInt32(buffer, 0);
                deplace = (result == 2);
            }
            catch (Exception) { retour = false; }

            if (supprimeLaListe)
                Clipboard.Clear();

            return retour;
        }
    }
}
