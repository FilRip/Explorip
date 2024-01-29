using System.Windows;

namespace Explorip.Constants;

internal static class Desktop
{
    public static GridLength ITEM_SIZE_X { get; private set; } = new(96, GridUnitType.Pixel);
    public static GridLength ITEM_SIZE_Y { get; private set; } = new(96, GridUnitType.Pixel);
}
