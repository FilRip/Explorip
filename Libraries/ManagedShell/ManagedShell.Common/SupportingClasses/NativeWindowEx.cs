using System.Windows.Forms;

namespace ManagedShell.Common.SupportingClasses
{
    public class NativeWindowEx : NativeWindow
    {
        public delegate void MessageReceivedEventHandler(Message m);

        public event MessageReceivedEventHandler MessageReceived;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            MessageReceived?.Invoke(m);
        }
    }
}
