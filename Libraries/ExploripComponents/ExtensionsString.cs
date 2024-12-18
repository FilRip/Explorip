using System;
using System.Linq;
using System.Text;

using ExploripConfig.Helpers;

namespace ExploripComponents;

public static class ExtensionsString
{
    /// <summary>
    /// Convert an Unicode string to an Ascii string<br/>
    /// Ignore char zero, 8206 and 8207 by défault
    /// </summary>
    /// <param name="uniString">Unicode string to convert</param>
    /// <param name="charsToExclude">Add char to exclude of the string</param>
    public static string ConvertFromUniToAscii(this string uniString, char[]? charsToExclude = null)
    {
        StringBuilder ret = new();
        foreach (char c in uniString)
        {
            if (c != '\0' && c != (char)8206 && c != (char)8207 && (charsToExclude == null || !charsToExclude.Contains(c)))
                ret.Append(c);
        }
        return ret.ToString();
    }

    public static string RemoveNotLetter(this string chaine)
    {
        return RemoveChars(chaine, "abcdefghijklmnopqrstuvwxyz".ToCharArray());
    }

    public static string RemoveNotDigit(this string chaine)
    {
        return RemoveChars(chaine, "0123456789".ToCharArray());
    }

    public static string RemoveNotDigitOrSeparator(this string chaine)
    {
        return RemoveChars(chaine, "0123456789 .,".ToCharArray());
    }

    public static string RemoveNotLetterOrDigit(this string chaine)
    {
        return RemoveChars(chaine, "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray());
    }

    public static string RemoveNotLetterOrDigitOrSeparator(this string chaine)
    {
        return RemoveChars(chaine, "abcdefghijklmnopqrstuvwxyz0123456789 .,".ToCharArray());
    }

    public static string RemoveChars(this string chaine, char[] caracteresAutorise)
    {
        if (chaine == null)
            throw new ArgumentNullException(nameof(chaine));
        return new string(chaine.ToCharArray().RemoveAll(caractere => !caracteresAutorise.Contains(caractere)));
    }
}
