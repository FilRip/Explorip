namespace CoolBytes.InteropWinRT.PriReader;

public abstract class ResourceMapEntry
{
    public ushort Index { get; }
    public ResourceMapScope? Parent { get; internal set; }
    public string Name { get; }

    internal ResourceMapEntry(ushort index, ResourceMapScope? parent, string name)
    {
        Index = index;
        Parent = parent;
        Name = name;
    }

    string? fullName;

    public string FullName
    {
        get
        {
            if (fullName == null)
                if (Parent == null)
                    fullName = Name;
                else
                    fullName = Parent.FullName + "\\" + Name;

            return fullName;
        }
    }
}
