using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

using CoolBytes.Helpers;
using CoolBytes.ScriptInterpreter.Enums;
using CoolBytes.ScriptInterpreter.Interfaces;
using CoolBytes.ScriptInterpreter.YAXLib;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class Serialization
{
    public static string Serialize(object objet, bool avecHeritage, int numMaxRecursion, YAXSerializationFields quoiLire)
    {
        string xmlObjet;
        YaxSerializer serialiseur = new(objet.GetType(),
                                        YAXExceptionHandlingPolicies.DoNotThrow,
                                        YAXExceptionTypes.Ignore,
                                        YAXSerializationOptions.SerializeNullObjects | YAXSerializationOptions.DisplayLineInfoInExceptions)
        {
            Heritage = avecHeritage,
            MaxRecursion = numMaxRecursion
        };
        serialiseur.ChangeTypeField(quoiLire);
        xmlObjet = serialiseur.Serialize(objet);
        return xmlObjet;
    }

    public static string SerializeDataContract(object objet, Type[] typeConnus)
    {
        DataContractSerializer serialiseur = new(objet.GetType(), typeConnus);
        MemoryStream ms = new();
        serialiseur.WriteObject(ms, objet);
        ms.Position = 0;
        StreamReader convertToString = new(ms); // Le constructeur dispose l'objet fourni en paramètre, inutile de le Disposer manuellement
        string retour = convertToString.ReadToEnd();
        convertToString.Dispose();
        return retour;
    }

    public static void ShowResult(IScriptReturn retour)
    {
        if (retour == null)
            return;
        if (string.IsNullOrWhiteSpace(retour.Filename))
        {
            try
            {
                string xmlObjet = "";
                if (retour.SerializationDataContract)
                {
                    Type[] typeConnus = null;
                    if (!string.IsNullOrWhiteSpace(retour.DataContractName))
                    {
                        Type type = ExtensionsAssembly.SearchTypeWithoutNamespace(retour.DataContractName);
                        if (type != null)
                        {
                            ServiceKnownTypeAttribute classeServiceKP = type.GetCustomAttribute<ServiceKnownTypeAttribute>();
                            if (classeServiceKP != null)
                            {
                                MethodInfo mi = classeServiceKP.DeclaringType.GetMethod(classeServiceKP.MethodName, BindingFlags.Static | BindingFlags.Public);
                                if (mi != null)
                                    try
                                    {
                                        typeConnus = (Type[])mi.Invoke(null, [null]);
                                    }
                                    catch { /* On ignore les erreurs qui peut survenir dans les méthodes abonnés à notre évènement, elles ne nous concernent pas */ }
                            }
                        }
                    }
                    xmlObjet = SerializeDataContract(retour.ObjectToSerialize, typeConnus);
                }
                else
                {
                    YAXSerializationFields quoiLire = YAXSerializationFields.None;
                    if (retour.WithFields)
                        quoiLire = YAXSerializationFields.AllFields;
                    if (retour.WithProperties)
                        quoiLire |= YAXSerializationFields.PublicProperties;
                    xmlObjet = Serialize(retour.ObjectToSerialize, retour.WithInherits, retour.MaxRecursiveInherits, quoiLire);
                }

                if (!string.IsNullOrWhiteSpace(xmlObjet))
                {
                    if (retour.CurrentPlatform == PlatformType.WPF)
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            WPF.WpfObject nouvelleFenetreObjet = new();
                            nouvelleFenetreObjet.LoadXml(xmlObjet);
                            nouvelleFenetreObjet.Show();
                        }));
                    }
                    else if (retour.CurrentPlatform == PlatformType.WINFORM)
                    {
                        Forms.FormObject nouvelleFenetreObjet = new();
                        nouvelleFenetreObjet.LoadXml(xmlObjet);
                        System.Windows.Forms.MethodInvoker invoqueShowForm = new(() => nouvelleFenetreObjet.Show());
                        System.Windows.Forms.Form.ActiveForm.BeginInvoke(invoqueShowForm);
                    }
                }
            }
            catch (Exception ex)
            {
                retour.TxtResult = Environment.NewLine + Properties.Resources.ERROR_SERIALIZATION + ex.Message + Environment.NewLine;
                if (System.Diagnostics.Debugger.IsAttached)
                    retour.TxtResult += ex.StackTrace.ToString();
            }
        }
        else
        {
            try
            {
                if (retour.CurrentPlatform == PlatformType.WPF)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        WPF.WpfObject nouvelleFenetreObjet = new();
                        nouvelleFenetreObjet.LoadFile(retour.Filename);
                        nouvelleFenetreObjet.Show();
                    }));
                }
                else if (retour.CurrentPlatform == PlatformType.WINFORM)
                {
                    Forms.FormObject nouvelleFenetreObjet = new();
                    nouvelleFenetreObjet.LoadFile(retour.Filename);
                    System.Windows.Forms.MethodInvoker invoqueShowForm = new(() => nouvelleFenetreObjet.Show());
                    System.Windows.Forms.Form.ActiveForm.BeginInvoke(invoqueShowForm);
                }
            }
            catch (Exception ex)
            {
                retour.TxtResult = Properties.Resources.ERROR_LOAD_STATE + ex.Message + Environment.NewLine;
            }
        }
    }
}
