using System.Windows;

namespace ExploripPlugins.Models;

public interface ITabExplorip
{
    public delegate void DelegateOnSelecting();
    public delegate void DelegateOnDeSelecting();

    /// <summary>
    /// Property - Set the Title of the Tab
    /// </summary>
    void SetTitle(string newTitle);

    /// <summary>
    /// Property - Set the Title of the current Window
    /// </summary>
    void SetWindowTitle(string newTitle);

    event DelegateOnSelecting OnSelecting;

    event DelegateOnSelecting OnDeSelecting;

    /// <summary>
    /// Occur when close/dispose the tab
    /// </summary>
    void Free();

    void TabDrop(object sender, DragEventArgs e);
}
