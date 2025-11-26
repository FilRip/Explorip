using System;
using System.Runtime.InteropServices;

using Microsoft.Toolkit.Uwp.Notifications;

namespace Explorip.TaskBar.Helpers;

#pragma warning disable CS0618
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("A1F7B5A2-0F7E-4F7D-9E7D-9B5E7C1F1234")]
public class MyNotificationActivator : NotificationActivator
{
    public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId)
    {
        // Ici tu peux gérer le clic sur la notification
        Console.WriteLine($"Toast activé avec arguments: {arguments}");
    }
}
#pragma warning restore CS0618
