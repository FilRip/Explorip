using System.Windows;

namespace Explorip.TaskBar.Helpers;

public static class WindowFlashingHelper
{
    public static readonly DependencyProperty IsFlashingProperty =
        DependencyProperty.RegisterAttached(
            nameof(IsFlashing),
            typeof(bool),
            typeof(WindowFlashingHelper),
            new FrameworkPropertyMetadata(false));

    public static void SetIsFlashing(DependencyObject element, bool value)
    {
        element.SetValue(IsFlashingProperty, value);
    }

    public static bool GetIsFlashing(DependencyObject element)
    {
        return (bool)element.GetValue(IsFlashingProperty);
    }

    public static bool IsFlashing { get; set; }
}
