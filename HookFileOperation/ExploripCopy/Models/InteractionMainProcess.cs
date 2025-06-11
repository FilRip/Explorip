using System;
using System.Collections.Generic;
using System.Linq;

using Explorip.HookFileOperations.Interfaces;
using Explorip.HookFileOperations.Models;

using ExploripCopy.ViewModels;

namespace ExploripCopy.Models;

[Serializable()]
internal class InteractionMainProcess : IInteractionMainProcess
{
    public void StartNewFileOperation(List<OneFileOperation> listOperations)
    {
        if (listOperations?.Count > 0)
        {
            // TODO : If RecycledBin file, get real name
            /*foreach (OneFileOperation operation in listOperations.Where(op => op.FileOperation == EFileOperation.Delete))
            {
                if (string.IsNullOrWhiteSpace(operation.DisplaySource))
                {
                    operation.SetDisplaySource(operation.Source);
                }
            }*/
            MainViewModels.Instance.AddOperations(listOperations);
            listOperations[listOperations.Count - 1].ResetChoice = true;
            MainViewModels.Instance.ForceUpdateWaitingList();
        }
    }
}
