namespace ExploripApi
{
    public interface IServerIpc
    {
        void ReceivedNewWindow(string[] args);

        void CreateFolder(string path, string name);

        //void CreateShortcut(string path, string name);
    }
}
