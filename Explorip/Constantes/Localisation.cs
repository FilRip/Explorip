using Explorip.Localization;

namespace Explorip.Constantes
{
    public class Localisation
    {
        public string LIBELLE_COLLER = "";
        public string LIBELLE_COLLER_RACCOURCI = "";

        private static Localisation _instance;

        public static Localisation GetInstance()
        {
            if (_instance == null)
                _instance = new Localisation();
            return _instance;
        }

        public Localisation()
        {
            LIBELLE_COLLER = GestionString.Load("shell32.dll", 33562, "Paste");
            LIBELLE_COLLER_RACCOURCI = GestionString.Load("shell32.dll", 37376, "Paste shortcut");
        }
    }
}
