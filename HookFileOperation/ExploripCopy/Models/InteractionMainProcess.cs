using System;
using System.Collections.Generic;

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
            MainViewModels.Instance.AddOperations(listOperations);
            listOperations[listOperations.Count - 1].ResetChoice = true;
            MainViewModels.Instance.ForceUpdateWaitingList();
        }
    }
}
