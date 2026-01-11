namespace CoolBytes.InteropWinRT.PriReader;

public struct DataItemRef
{
    internal SectionRef<DataItemSection> dataItemSection;
    internal int itemIndex;

    internal DataItemRef(SectionRef<DataItemSection> dataItemSection, int itemIndex)
    {
        this.dataItemSection = dataItemSection;
        this.itemIndex = itemIndex;
    }

    public override readonly string ToString()
    {
        return $"Data item {itemIndex} in section {dataItemSection.sectionIndex}";
    }
}
