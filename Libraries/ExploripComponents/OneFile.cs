using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.Common.Enums;

namespace ExploripComponents;

public partial class OneFile : OneFileSystem
{
    [ObservableProperty()]
    private IconSize _iconSize;
}
