using CoolBytes.ScriptInterpreter.Enums;

namespace CoolBytes.ScriptInterpreter.Interfaces;

public interface IScriptReturn
{
    string Filename { get; set; }
    object ObjectToSerialize { get; set; }
    bool SerializationDataContract { get; }
    string DataContractName { get; }
    bool WithInherits { get; }
    bool WithProperties { get; }
    bool WithFields { get; }
    int MaxRecursiveInherits { get; }
    PlatformType CurrentPlatform { get; }
    string TxtResult { get; set; }
    void OpenWindow();
    bool OpenInNewWindow { get; set; }
}
