using ManagedShell.Common.Enums;

namespace ManagedShell.Common.Structs;

internal struct StartupLocation
{
    internal StartupEntryType Type;
    internal string Location;
    internal string ApprovedLocation;
    internal StartupEntryScope Scope;
}
