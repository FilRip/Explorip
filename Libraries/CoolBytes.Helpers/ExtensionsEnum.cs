using System;
using System.ComponentModel;
using System.Reflection;

namespace CoolBytes.Helpers;

public static class ExtensionsEnum
{
    public static string GetEnumDescription<T>(T enumObj, string defaultString = null)
    {
        string result = string.Empty;

        if (enumObj is not null)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                DescriptionAttribute descriptionAttrib = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

                if (descriptionAttrib == null)
                    result = (string.IsNullOrWhiteSpace(defaultString) ? enumObj.ToString() : defaultString);
                else
                    result = descriptionAttrib.Description;
            }
        }
        return result;
    }

    public static T GetValueFromDescription<T>(string description) where T : Enum
    {
        foreach (var value in typeof(T).GetEnumValues())
            if (GetEnumDescription(typeof(T), value.ToString()) == description)
                return (T)value;
        return default;
    }
}
