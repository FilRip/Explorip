using System;
using System.Text;
using System.Threading;

using CoolBytes.Scripting.Models;
using CoolBytes.ScriptInterpreter.Interfaces;

using Microsoft.CodeAnalysis;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class Treatments
{
    public static void TreatResult(IScriptReturn destination, ScriptingResult scriptReturn, bool openWindow)
    {
        if (scriptReturn != null)
        {
            if (scriptReturn.Success)
            {
                destination.TxtResult = Properties.Resources.SCRIPT_OK_RETURN;
                destination.TxtResult += scriptReturn.Result == null ? Properties.Resources.SCRIPT_RETURN_VOID : scriptReturn.Result.ToString();
                if (scriptReturn.Result != null && !scriptReturn.Result.GetType().IsPrimitive && scriptReturn.Result is not string && scriptReturn.Result is not Enum && scriptReturn.Result is not DateTime && scriptReturn.Result is not DateTimeOffset && scriptReturn.Result is not TimeSpan && openWindow)
                {
                    destination.Filename = null;
                    destination.ObjectToSerialize = scriptReturn.Result;
                    Thread monThread = new(new ThreadStart(destination.OpenWindow));
                    monThread.Start();
                }
            }
            else
            {
                StringBuilder sb = new();
                sb.AppendLine(Properties.Resources.ERRORS);
                if (scriptReturn.Errors != null)
                    foreach (Diagnostic erreur in scriptReturn.Errors)
                        sb.AppendLine(erreur.ToString());
                if (scriptReturn.ExceptionThrowed != null)
                    sb.AppendLine(scriptReturn.ExceptionThrowed?.ToString());
                destination.TxtResult = sb.ToString();
            }
        }
    }
}
