using System;

namespace CoolBytes.InteropWinRT.PriReader.Constants;

[Flags()]
public enum EPriDescriptors : ushort
{
    AutoMerge = 1,
    IsDeploymentMergeable = 2,
    IsDeploymentMergeResult = 4,
    IsAutomergeMergeResult = 8,
}
