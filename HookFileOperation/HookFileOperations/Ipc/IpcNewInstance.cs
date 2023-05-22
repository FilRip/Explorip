using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Explorip.HookFileOperations.Interfaces;
using Explorip.HookFileOperations.Models;

namespace Explorip.HookFileOperations.Ipc
{
    public class IpcNewInstance : MarshalByRefObject
    {
        private IInteractionMainProcess _interactionWithMainProcess;

        public void SetMainProcess(IInteractionMainProcess interactionWithMainProcess)
        {
            _interactionWithMainProcess = interactionWithMainProcess;
        }

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetDesktopWindow();

        public void StartNewFileOperation(List<OneFileOperation> listOperations)
        {
            Task.Run(() =>
            {
                if (_interactionWithMainProcess == null)
                {
                    Console.WriteLine("PerformOperation in dedicated process");
                    FileOperation currentFileOperation = new(GetDesktopWindow());
                    foreach (OneFileOperation ope in listOperations)
                        ope.WriteOperation(currentFileOperation);
                    currentFileOperation.PerformOperations();
                    Console.WriteLine("End operation");
                    Environment.Exit(0);
                }
                else
                {
                    _interactionWithMainProcess.StartNewFileOperation(listOperations);
                }
            });
        }

#pragma warning disable S3400 // Methods should not return constants
        public bool IsReady()
        {
            return true;
        }
#pragma warning restore S3400 // Methods should not return constants
    }
}
