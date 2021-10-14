using System.Collections;

namespace Explorip.Sorters
{
    public class TriAlphabetique : IComparer
    {
        public int Compare(object x, object y)
        {
            return x.ToString().CompareTo(y.ToString());
        }
    }
}
