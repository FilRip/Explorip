namespace CoolBytes.InteropWinRT.PriReader;

public struct ResourceMapItemRef
{
    internal SectionRef<HierarchicalSchemaSection> schemaSection;
    internal int itemIndex;

    internal ResourceMapItemRef(SectionRef<HierarchicalSchemaSection> schemaSection, int itemIndex)
    {
        this.schemaSection = schemaSection;
        this.itemIndex = itemIndex;
    }
}
