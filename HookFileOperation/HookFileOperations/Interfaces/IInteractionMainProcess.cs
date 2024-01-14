using System.Collections.Generic;

using Explorip.HookFileOperations.Models;

namespace Explorip.HookFileOperations.Interfaces;

public interface IInteractionMainProcess
{
    void StartNewFileOperation(List<OneFileOperation> listOperations);
}
