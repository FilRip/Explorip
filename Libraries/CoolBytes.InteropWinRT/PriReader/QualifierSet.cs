using System.Collections.Generic;

namespace CoolBytes.InteropWinRT.PriReader;

public record struct QualifierSet(ushort Index, IReadOnlyList<Qualifier> Qualifiers);
