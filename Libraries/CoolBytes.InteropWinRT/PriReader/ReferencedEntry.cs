namespace CoolBytes.InteropWinRT.PriReader;

public abstract class ReferencedEntry(ReferencedFolder? parent, string name)
{
    public ReferencedFolder? Parent { get; internal set; } = parent;
    public string Name { get; } = name;

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
