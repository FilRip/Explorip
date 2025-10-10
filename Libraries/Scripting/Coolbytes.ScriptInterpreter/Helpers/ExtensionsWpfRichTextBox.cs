using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class ExtensionsWpfRichTextBox
{
    public static string Text(this RichTextBox rtb)
    {
        return rtb.InvokeIfRequired(() => { return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text; });
    }

    public static string[] Lines(this RichTextBox rtb)
    {
        return rtb.Text().Split('\n');
    }

    public static int SelectionStart(this RichTextBox rtb)
    {
        return rtb.InvokeIfRequired(() => { return rtb.Document.ContentStart.GetOffsetToPosition(rtb.CaretPosition); });
    }

    public static Rect GetRectFromCharacterIndex(this RichTextBox rtb, int caretPosition)
    {
        return rtb.InvokeIfRequired(() => { return rtb.Document.ContentStart.GetPositionAtOffset(caretPosition).GetCharacterRect(LogicalDirection.Forward); });
    }

    public static int GetLineIndexFromCharacterIndex(this RichTextBox rtb, int caretPosition)
    {
        return Math.Abs(rtb.InvokeIfRequired(() =>
        {
            rtb.Document.ContentStart.GetPositionAtOffset(caretPosition).GetLineStartPosition(-int.MaxValue, out int retour);
            return retour;
        }));
    }

    public static string GetLineText(this RichTextBox rtb, int numLigne)
    {
        try
        {
            return rtb.Lines()[numLigne];
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static int SelectionLength(this RichTextBox rtb)
    {
        return rtb.InvokeIfRequired(() => { return rtb.Selection.Text.Length; });
    }
}
