using System;

namespace Explorip.Helpers
{
    public static class ExtensionsArray
    {
        public static T[] RemoveRange<T>(this T[] liste, int index, int nb)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (index > liste.Length - 1) throw new IndexOutOfRangeException();

            T[] retour = liste;
            for (int i = index; i < index + nb; i++)
                if (i < retour.Length)
                    retour = retour.RemoveAt(index);
            return retour;
        }

        public static T[] RemoveAt<T>(this T[] liste, int index)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (index >= liste.Length) throw new IndexOutOfRangeException();
            if (liste.Length == 0) throw new IndexOutOfRangeException();

            T[] newListe = new T[liste.Length - 1];
            if (liste.Length > 0)
                for (int i = 0; i < liste.Length; i++)
                    if (i != index) newListe[(i > index ? i - 1 : i)] = liste[i];

            return newListe;
        }

        public static T[] Remove<T>(this T[] liste, object objetASupprimer, bool toutesLesOccurences = true)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (objetASupprimer == null) throw new ArgumentNullException(nameof(objetASupprimer));

            if (liste.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < liste.Length; i++)
                        if ((liste[i] != null) && (liste[i].Equals(objetASupprimer)))
                        {
                            if (toutesLesOccurences) trouve = true;
                            liste = liste.RemoveAt(i);
                            break;
                        }
                }
            }

            return liste;
        }

        public static T[] RemoveAllNull<T>(this T[] liste)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));

            if (liste.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < liste.Length; i++)
                        if (liste[i] == null)
                        {
                            trouve = true;
                            liste = liste.RemoveAt(i);
                            break;
                        }
                }
            }

            return liste;
        }

        public static T[] Add<T>(this T[] liste, object objetAAjouter)
        {
            if (objetAAjouter == null) throw new ArgumentNullException(nameof(objetAAjouter));

            if ((typeof(T) == objetAAjouter.GetType()) || (objetAAjouter.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref liste, liste.Length + 1);
                liste[liste.Length - 1] = (T)objetAAjouter;
            }
            return liste;
        }

        public static T[] Insert<T>(this T[] liste, object objetAInserer, int position)
        {
            if (objetAInserer == null) throw new ArgumentNullException(nameof(objetAInserer));

            if (position > liste.Length - 1) position = liste.Length;
            if ((typeof(T) == objetAInserer.GetType()) || (objetAInserer.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref liste, liste.Length + 1);
                for (int i = liste.Length - 1; i > position; i--)
                    liste[i] = liste[i - 1];
                liste[position] = (T)objetAInserer;
            }
            return liste;
        }

        public static string HexToString(this byte[] buffer)
        {
            return HexToString(buffer, ",");
        }

        public static string HexToString(this byte[] buffer, string separateur)
        {
            if ((buffer == null) || (buffer.Length == 0))
                return "";
            string retour = "";
            foreach (byte octet in buffer)
            {
                if (retour != "")
                    retour += separateur;
                retour += "0x" + octet.ToString("X");
            }
            return retour;
        }
    }
}
