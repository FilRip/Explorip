using System.Windows.Media;

namespace ExploripComponents;

public class OneDirectory : OneFileSystem
{
    public ImageSource Icon { get; set; }

    public ImageSource IconOverlay { get; set; }

    public bool Opened { get; set; }

    public bool HasChildren { get; set; }
}
