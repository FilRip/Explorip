using System.Windows.Interop;

namespace VirtualDesktop.Utils;

internal class TransparentWindow : RawWindow
{
    public override void Show()
    {
        HwndSourceParameters parameters = new(this.Name)
        {
            Width = 1,
            Height = 1,
            WindowStyle = 0x800000,
        };

        this.Show(parameters);
    }
}
