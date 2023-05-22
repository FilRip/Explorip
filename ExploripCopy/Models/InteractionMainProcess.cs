using System;
using System.Collections.Generic;

using Explorip.HookFileOperations.Interfaces;
using Explorip.HookFileOperations.Models;

using ExploripCopy.ViewModels;

namespace ExploripCopy.Models
{
    [Serializable()]
    internal class InteractionMainProcess : IInteractionMainProcess
    {
        public void StartNewFileOperation(List<OneFileOperation> listOperations)
        {
            MainViewModels.Instance.ListWaiting.AddRange(listOperations);
            MainViewModels.Instance.ForceUpdateWaitingList();
        }
    }
}
