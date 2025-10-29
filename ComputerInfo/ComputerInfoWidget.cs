using System;
using System.Windows.Controls;

using ExploripPlugins;

namespace ComputerInfo
{
    public class ComputerInfoWidget : IExploripDesktop
    {
        public string Author => "FilRip";

        public string Name => "ComputerInfo";

        public string Description => "Give some informations in real time";

        public Version Version => new(1, 0, 0, 0);

        public UserControl ExploripWidget => throw new NotImplementedException();

        public Guid GuidKey => new("{4A02DC97-4AF5-4BDA-88EA-C93A4FA96181}");
    }
}
