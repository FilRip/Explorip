using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Explorip.HookFileOperations.FilesOperations;
using Explorip.HookFileOperations.Interfaces;
using Explorip.HookFileOperations.Models;

using ExploripApi;

namespace Explorip.HookFileOperations.Ipc;

public class IpcNewInstance : MarshalByRefObject
{
    private IInteractionMainProcess _interactionWithMainProcess;

    public void SetMainProcess(IInteractionMainProcess interactionWithMainProcess)
    {
        _interactionWithMainProcess = interactionWithMainProcess;
    }

    public bool ExploripCopyLaunched()
    {
        return _interactionWithMainProcess != null;
    }

    [DllImport("USER32.DLL")]
    private static extern IntPtr GetDesktopWindow();

    public void StartNewFileOperation(List<OneFileOperation> listOperations)
    {
        Task.Run(() =>
        {
            if (_interactionWithMainProcess == null ||
                (listOperations.Count == 1 && listOperations[0].FileOperation == Models.EFileOperation.Create))
            {
                try
                {
                    if (listOperations.Count == 1 && listOperations[0].FileOperation == Models.EFileOperation.Create)
                    {
                        if (listOperations[0].Attributes.HasFlag(FileAttributes.Directory))
                        {
                            IpcServerManager.CreateFolder(listOperations[0].Destination, listOperations[0].NewName);
                            return;
                        }
                        if (Path.GetExtension(listOperations[0].NewName).ToLower() == ".lnk")
                        {
                            IpcServerManager.CreateShortcut(listOperations[0].Destination, listOperations[0].NewName);
                            return;
                        }
                    }
                    FileOperation currentFileOperation = new(GetDesktopWindow());
                    foreach (OneFileOperation ope in listOperations)
                        ope.WriteOperation(currentFileOperation);
                    currentFileOperation.PerformOperations();
                    currentFileOperation.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during HookCopy : {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
                finally
                {
                    if (_interactionWithMainProcess == null)
                        Environment.Exit(0);
                }
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
