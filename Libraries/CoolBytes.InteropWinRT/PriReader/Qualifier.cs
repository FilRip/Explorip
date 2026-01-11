using CoolBytes.InteropWinRT.PriReader.Constants;

namespace CoolBytes.InteropWinRT.PriReader;

public record struct Qualifier(ushort Index, EQualifierType Type, ushort Priority, float FallbackScore, string Value);
