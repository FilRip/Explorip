using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using CoolBytes.ScriptInterpreter.Models;

namespace CoolBytes.ScriptInterpreter.Interfaces;

public interface IIntellisense
{
    bool ListVisible { get; set; }
    bool ActiveIntellisense { get; }
    string TxtScript();
    int PosCursor { get; set; }
    Color NamespaceColor { get; }
    Color MethodsColor { get; }
    Color CommentaryColor { get; }
    Color PropertiesColor { get; }
    Color FieldsColor { get; }
    void SelectText(int start, int end);
    void SetSelectionColor(Color newColor);
    void KeyboardInput(char key);
    void KeyboardInputIntellisense(KeyEventArgs e = null, System.Windows.Input.KeyEventArgs eWPF = null);
    string ReturnCurrentLine { get; }
    int ReturnCurrentNumLine { get; }
    void FillIntellisenseList(List<OneElementType> list);
    int PositionList { get; set; }
    MethodInfo[] CurrentMethods { get; set; }
    int NumOverrides { get; set; }
    bool IgnoreCase { get; }
    bool InternalDebug { get; }
    void ResetPosIntellisense();
    string StartClassType { get; }
    List<OneElementType> ListIntellisense { get; set; }
    string FilterScript { get; set; }
    Color WriteColor { get; }
    Color SelectionColor { get; }
    int PosFirstCharCurrentLine { get; }
    int PosLastCharCurrentLine { get; }
    char ReadChar(int index);
    string ReadString(int start, int length);
    bool SearchInReflection { get; set; }
}
