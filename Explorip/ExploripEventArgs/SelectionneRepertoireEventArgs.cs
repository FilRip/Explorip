using System.IO;

namespace Explorip.ExploripEventArgs
{
    public class SelectionneRepertoireEventArgs : System.EventArgs
    {
        private readonly DirectoryInfo _dirInfo;

        public SelectionneRepertoireEventArgs(DirectoryInfo dirInfo)
        {
            _dirInfo = dirInfo;
        }

        public DirectoryInfo DirInfo
        {
            get { return _dirInfo; }
        }
    }
}
