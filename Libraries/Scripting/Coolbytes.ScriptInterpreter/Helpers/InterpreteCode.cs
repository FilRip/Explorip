using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using CoolBytes.Helpers;
using CoolBytes.ScriptInterpreter.Interfaces;
using CoolBytes.ScriptInterpreter.Models;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class InterpreteCode
{
    public static bool KeyboardInputIntellisense(IIntellisense intellisense, Key e)
    {
        if (intellisense.ActiveIntellisense && intellisense.ListVisible &&
            ((e == Key.Down) ||
                (e == Key.Up) ||
                (e == Key.Enter) ||
                (e == Key.Return) ||
                (e == Key.Tab) ||
                (e == Key.PageDown) ||
                (e == Key.PageUp) ||
                (e == Key.Home) ||
                (e == Key.End)))
        {
            switch (e)
            {
                case Key.Down:
                    if ((intellisense.CurrentMethods != null) && (intellisense.CurrentMethods.Length > 0) && (intellisense.NumOverrides < intellisense.CurrentMethods.Length - 1))
                    {
                        intellisense.NumOverrides++;
                        List<OneElementType> listeChamps = [];
                        intellisense.CurrentMethods[intellisense.NumOverrides].GetParameters().ToList().ForEach(item => listeChamps.Add(new OneElementType(item.Name, item.ParameterType.Name, item.IsOptional, item.DefaultValue.ToString())));
                        intellisense.FillIntellisenseList(listeChamps);
                    }
                    else if (intellisense.CurrentMethods == null)
                        intellisense.PositionList++;
                    break;
                case Key.Up:
                    if ((intellisense.CurrentMethods != null) && (intellisense.CurrentMethods.Length > 0) && (intellisense.NumOverrides > 0))
                    {
                        intellisense.NumOverrides--;
                        List<OneElementType> listeChamps = [];
                        intellisense.CurrentMethods[intellisense.NumOverrides].GetParameters().ToList().ForEach(item => listeChamps.Add(new OneElementType(item.Name, item.ParameterType.Name, item.IsOptional, item.DefaultValue.ToString())));
                        intellisense.FillIntellisenseList(listeChamps);
                    }
                    else if (intellisense.CurrentMethods == null)
                        intellisense.PositionList--;
                    break;
                case Key.PageDown:
                    intellisense.PositionList += 4;
                    break;
                case Key.PageUp:
                    intellisense.PositionList -= 4;
                    break;
                case Key.Home:
                    intellisense.PositionList = 0;
                    break;
                case Key.End:
                    intellisense.PositionList = int.MaxValue;
                    break;
            }
            return true;
        }
        return false;
    }

    public static void KeyboardInput(IIntellisense intellisense, char key)
    {
        Thread thread = new(new ParameterizedThreadStart(ThreadKeyboardInput));
        thread.Start(new object[] { intellisense, key });
    }

    private static void ThreadKeyboardInput(object state)
    {
        object[] listParams = (object[])state;
        IIntellisense intellisense = (IIntellisense)listParams[0];
        char key = (char)listParams[1];

        try
        {
            if (!intellisense.ActiveIntellisense)
                return;

            string ns;
            int cursorPos = intellisense.PosCursor;
            ns = intellisense.ReturnCurrentLine.Trim();
            if (Array.Exists(StartComments.ListStartComments(intellisense.IgnoreCase), comm => ns.StartsWith(comm)))
            {
                intellisense.SelectText(intellisense.PosFirstCharCurrentLine, intellisense.PosLastCharCurrentLine);
                intellisense.SetSelectionColor(intellisense.CommentaryColor);
                intellisense.PosCursor = cursorPos;
                intellisense.SelectText(cursorPos, cursorPos);
                return;
            }
            if (intellisense.SelectionColor == intellisense.CommentaryColor)
            {
                intellisense.SelectText(intellisense.PosFirstCharCurrentLine, intellisense.PosLastCharCurrentLine);
                intellisense.SetSelectionColor(intellisense.WriteColor);
                intellisense.PosCursor = cursorPos;
                intellisense.SelectText(cursorPos, cursorPos);
            }

            if ((key == '.') || (key == '('))
                try
                {
                    intellisense.FillIntellisenseList(null);
                    intellisense.ListVisible = false;
                    intellisense.FilterScript = "";
                    intellisense.NumOverrides = 0;
                    intellisense.CurrentMethods = null;

                    BindingFlags portee = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                    List<OneElementType> listeParams = null;

                    ns = intellisense.ReturnCurrentLine.Trim();

                    if (ns.IndexOf(' ', 0, Math.Min(intellisense.PosCursor - intellisense.PosFirstCharCurrentLine, ns.Length)) >= 0)
                        ns = ns.Substring(ns.LastIndexOf(' ') + 1, ns.Length - ns.LastIndexOf(' ') - 1);

                    if ((Math.Min(intellisense.PosCursor - intellisense.PosFirstCharCurrentLine, ns.Length) + 1 < ns.LastIndexOf('(')) && (ns.IndexOf('(', 0, Math.Min(intellisense.PosCursor - intellisense.PosFirstCharCurrentLine, ns.Length)) >= 0))
                        ns = ns.Substring(ns.LastIndexOf('(') + 1, ns.Length - ns.LastIndexOf('(') - 1);

                    if (ns.EndsWith("."))
                        ns = ns.Substring(0, ns.Length - 1);

                    if (ns != "")
                    {
                        Type type = AppDomain.CurrentDomain.SearchTypeWithoutNamespace(ns, intellisense.IgnoreCase, intellisense.SearchInReflection);
                        if ((type == null) && (intellisense.StartClassType != "") && (!ns.StartsWith(intellisense.StartClassType, intellisense.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
                        {
                            ns = intellisense.StartClassType + "." + ns;
                            type = AppDomain.CurrentDomain.SearchTypeWithoutNamespace(ns, intellisense.IgnoreCase, intellisense.SearchInReflection);
                        }
                        if ((type == null) && (ns.IndexOf('.') >= 0))
                        {
                            if (intellisense.StartClassType != "")
                                ns = ns.Replace(intellisense.StartClassType + ".", "");
                            string[] listNs = ns.Split('.');
                            StringBuilder sb = new();
                            foreach (string sousNs in listNs)
                            {
                                if (string.IsNullOrWhiteSpace(sousNs))
                                    break;
                                if (sb.Length > 0)
                                    sb.Append(".");
                                sb.Append(sousNs);
                                type = AppDomain.CurrentDomain.SearchType(sb.ToString(), intellisense.IgnoreCase, intellisense.SearchInReflection);
                                if (type != null)
                                    break;
                            }
                            if ((type != null) && (key == '('))
                            {
                                string nomMethode = listNs[listNs.Length - 1];
                                intellisense.CurrentMethods = [.. type.GetMethods(portee).Where(item => string.Compare(item.Name, nomMethode, intellisense.IgnoreCase) == 0)];
                                if ((intellisense.CurrentMethods != null) && (intellisense.CurrentMethods.Length > 0))
                                {
                                    listeParams = [];
                                    intellisense.CurrentMethods[0].GetParameters().ToList().ForEach(item => listeParams.Add(new OneElementType(item.Name, item.ParameterType.Name, item.IsOptional, item.DefaultValue.ToString())));
                                    intellisense.NumOverrides = 0;
                                }
                            }
                            else
                            {
                                type = null;
                                if (intellisense.StartClassType != "")
                                    ns = intellisense.StartClassType + "." + ns;
                            }
                        }
                        if ((type == null) && (ns.IndexOf('.') >= 0))
                        {
                            string[] listFields;
                            listFields = ns.Split('.');
                            type = AppDomain.CurrentDomain.SearchTypeWithoutNamespace(listFields[0], intellisense.IgnoreCase, intellisense.SearchInReflection);
                            if (type != null)
                            {
                                listeParams = null;
                                listFields = listFields.RemoveAt(0);

                                if (listFields[listFields.Length - 1] == "")
                                    listFields = listFields.RemoveAt(listFields.Length - 1);

                                FieldInfo fi;
                                PropertyInfo pi;
                                MethodInfo mi;
                                string miWithoutParam;
                                foreach (string fieldName in listFields)
                                {
                                    if (type == null)
                                        break;
                                    pi = Array.Find(type.GetProperties(portee), item => string.Compare(item.Name, fieldName.Trim(), intellisense.IgnoreCase) == 0);
                                    if (pi != null)
                                        type = pi.PropertyType;
                                    else
                                    {
                                        fi = Array.Find(type.GetFields(portee), item => string.Compare(item.Name, fieldName.Trim(), intellisense.IgnoreCase) == 0);
                                        if (fi != null)
                                            type = fi.FieldType;
                                        else
                                        {
                                            miWithoutParam = fieldName;
                                            if (miWithoutParam.IndexOf('(') >= 0)
                                                miWithoutParam = miWithoutParam.Substring(0, miWithoutParam.IndexOf('('));

                                            mi = Array.Find(type.GetMethods(portee), item => string.Compare(item.Name, miWithoutParam, intellisense.IgnoreCase) == 0);
                                            if (mi != null)
                                            {
                                                if (key == '(')
                                                {
                                                    intellisense.CurrentMethods = [.. type.GetMethods(portee).Where(item => string.Compare(item.Name, miWithoutParam, intellisense.IgnoreCase) == 0)];
                                                    if ((intellisense.CurrentMethods != null) && (intellisense.CurrentMethods.Length > 0))
                                                    {
                                                        listeParams = [];
                                                        intellisense.CurrentMethods[0].GetParameters().ToList().ForEach(item => listeParams.Add(new OneElementType(item.Name, item.ParameterType.Name, item.IsOptional, item.DefaultValue.ToString())));
                                                        intellisense.NumOverrides = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (mi.ReturnType != typeof(void))
                                                        type = mi.ReturnType;
                                                    else
                                                        type = null;
                                                }
                                            }
                                            else
                                                type = null;
                                        }
                                    }
                                }
                            }
                        }
                        if (type != null)
                        {
                            if (listeParams != null)
                            {
                                if (listeParams.Count == 0) listeParams.Add(new OneElementType("null", ElementTypes.PARAMETER));
                                intellisense.FillIntellisenseList(listeParams);
                                intellisense.ListVisible = true;
                            }
                            else
                            {
                                if (intellisense.ListIntellisense != null)
                                    intellisense.ListIntellisense.Clear();
                                else
                                    intellisense.ListIntellisense = [];

                                type.GetFields(portee).ToList().ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Name, ElementTypes.FIELDS)));
                                type.GetProperties(portee).ToList().ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Name, ElementTypes.PROPERTIES)));
                                type.GetMethods(portee).Where(item => !item.IsSpecialName).ToList().ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Name, ElementTypes.METHOD)));
                                intellisense.ListIntellisense = [.. intellisense.ListIntellisense.OrderBy(item => item.Name)];
                                intellisense.ListIntellisense = [.. intellisense.ListIntellisense.GroupBy(item => item.Name).Select(sel => sel.First())];
                                intellisense.FillIntellisenseList(intellisense.ListIntellisense);
                                intellisense.ResetPosIntellisense();
                                intellisense.ListVisible = true;
                            }
                        }
                        else
                        {
                            if (intellisense.ListIntellisense != null)
                                intellisense.ListIntellisense.Clear();
                            else
                                intellisense.ListIntellisense = [];

                            if (intellisense.StartClassType != "")
                                ns = ns.Replace(intellisense.StartClassType + ".", "");

                            List<string> listNameSpace = [.. ExtensionsAssembly.ListAllNamespace(AppDomain.CurrentDomain, intellisense.SearchInReflection).Where(item => item.StartsWith(ns + '.'))];
                            List<Type> listTypes = [.. ExtensionsAssembly.ListAllTypes(AppDomain.CurrentDomain, intellisense.SearchInReflection).Where(item => (!item.Name.Contains('`')) && item.IsPublic && (!item.Assembly.IsDynamic) && item.IsClass && (!string.IsNullOrWhiteSpace(item.Namespace)) && item.Namespace.StartsWith(ns, intellisense.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))];
                            if ((listNameSpace != null) && (listNameSpace.Count > 0))
                                listNameSpace.ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Substring(ns.Length + 1), ElementTypes.NAMESPACE)));

                            listTypes?.ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Name, ElementTypes.NAMESPACE)));

                            Type classType = ExtensionsAssembly.SearchType(ns);
                            classType?.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(item => !item.IsSpecialName).ToList().ForEach(item => intellisense.ListIntellisense.Add(new OneElementType(item.Name, ElementTypes.METHOD)));

                            if (intellisense.ListIntellisense.Count > 0)
                            {
                                intellisense.ListIntellisense = [.. intellisense.ListIntellisense.OrderBy(item => item.Name)];
                                intellisense.ListIntellisense = [.. intellisense.ListIntellisense.GroupBy(item => item.Name).Select(sel => sel.First())];
                                intellisense.FillIntellisenseList(intellisense.ListIntellisense);
                                intellisense.ListVisible = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (intellisense.InternalDebug)
                        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            else if (key == ')')
                intellisense.ListVisible = false;
            else if (((char.IsLetterOrDigit(key) || (key == (char)8)) &&
                     intellisense.ListVisible &&
                     (intellisense.ListIntellisense != null) &&
                     (intellisense.ListIntellisense.Count > 0) &&
                     (!intellisense.ListIntellisense.Exists(item => item.ElementType == ElementTypes.PARAMETER))) &&
                     intellisense.PosCursor > 0)
            {
                int start = intellisense.PosCursor;
                while (intellisense.TxtScript()[start - 1] == '\n' || intellisense.TxtScript()[start - 1] == '\r' || char.IsLetterOrDigit(intellisense.ReadChar(start - 1)) && (start > 0))
                    start--;
                intellisense.FilterScript = intellisense.ReadString(start, intellisense.PosCursor - start).Trim();
                if ((intellisense.ListIntellisense != null) && (intellisense.ListIntellisense.Count > 0))
                    intellisense.FillIntellisenseList([.. intellisense.ListIntellisense.Where(item => item.Name.Contains(intellisense.FilterScript, intellisense.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))]);
            }
        }
        catch (Exception ex)
        {
            if (intellisense.InternalDebug)
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}
