using System;

namespace Explorip.Helpers
{
    public static class ExtensionsInt
    {
        public static bool EtatBit(this int integer, int position)
        {
            int bit = (int)Math.Pow(2,(position - 1));
            return (integer & bit) == bit;
        }
    }
}
