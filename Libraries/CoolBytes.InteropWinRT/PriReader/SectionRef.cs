namespace CoolBytes.InteropWinRT.PriReader;

public struct SectionRef<T> where T : Section
{
    internal int sectionIndex;

    internal SectionRef(int sectionIndex)
    {
        this.sectionIndex = sectionIndex;
    }

    public override readonly string ToString()
    {
        return $"Section {typeof(T).Name} at index {sectionIndex}";
    }
}
