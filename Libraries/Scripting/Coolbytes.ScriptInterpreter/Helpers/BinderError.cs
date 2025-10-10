namespace CoolBytes.ScriptInterpreter.Helpers;

internal class BinderError
{
    internal bool IsChecked { get; set; }
    internal bool HasError { get; set; }
    internal string Message { get; set; }

    internal BinderError()
    {
        IsChecked = false;
        HasError = false;
        Message = string.Empty;
    }

    internal BinderError(string message)
    {
        SetError(message);
    }

    internal void SetError(string message)
    {
        IsChecked = true;
        HasError = true;
        Message = message;
    }

    internal void RemoveError()
    {
        IsChecked = true;
        HasError = false;
        Message = string.Empty;
    }
}
