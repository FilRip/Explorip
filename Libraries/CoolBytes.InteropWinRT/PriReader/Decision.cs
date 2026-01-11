using System.Collections.Generic;

namespace CoolBytes.InteropWinRT.PriReader;

public record struct Decision(ushort Index, IReadOnlyList<QualifierSet> QualifierSets);
