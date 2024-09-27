using System.Windows.Media;

namespace ExploripComponents;

public abstract class OneFileSystem
{
    public string Text { get; set; }

    public SolidColorBrush Foreground { get; set; } = new SolidColorBrush(Colors.White);

    public SolidColorBrush Background { get; set; } = new SolidColorBrush(Colors.Transparent);

    public bool Selected { get; set; }
}
