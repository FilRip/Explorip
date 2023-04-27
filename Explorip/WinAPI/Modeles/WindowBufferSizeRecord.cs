namespace Explorip.WinAPI.Modeles
{
    public struct WindowBufferSizeRecord
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public Coord dwSize;
#pragma warning restore S1104 // Fields should not have public accessibility

        public WindowBufferSizeRecord(short x, short y)
        {
            this.dwSize = new Coord(x, y);
        }
    }
}
