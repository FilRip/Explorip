namespace ManagedShell.ShellFolders.Enums;

public enum SORTDIRECTION
{
    /// <summary>
    /// The items are sorted in ascending order. Whether the sort is alphabetical, numerical, and so on, is determined by the data
    /// type of the column indicated in <see cref="SORTCOLUMN.propkey"/>.
    /// </summary>
    SORT_DESCENDING = -1,

    /// <summary>
    /// The items are sorted in descending order. Whether the sort is alphabetical, numerical, and so on, is determined by the data
    /// type of the column indicated in <see cref="SORTCOLUMN.propkey"/>.
    /// </summary>
    SORT_ASCENDING = 1
}
