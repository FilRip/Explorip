using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CoolBytes.Helpers;

public static class ExtensionsString
{
    public static string RemoveString(this string str, string characters)
    {
        return str.Replace(characters, "");
    }

    public static string RemoveChars(this string str, char[] characters)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (characters == null)
            throw new ArgumentNullException(nameof(characters));
        if (characters.Length == 0)
            return str;
        foreach (char c in characters)
            str = str.RemoveChar(c);
        return str;
    }

    public static string RemoveNotLetter(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "abcdefghijklmnopqrstuvwxyz";
        validChars += validChars.ToUpper();
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveNotDigit(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "0123456789";
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveNotDigitOrSeparator(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "0123456789 ,.";
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveNotLetterOrDigit(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "abcdefghijklmnopqrstuvwxyz";
        validChars += validChars.ToUpper();
        validChars += "0123456789";
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveNotHexLetterOrDigit(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "abcdef";
        validChars += validChars.ToUpper();
        validChars += "0123456789";
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveNotLetterOrDigitOrSeparator(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        string validChars = "abcdefghijklmnopqrstuvwxyz";
        validChars += validChars.ToUpper();
        validChars += "0123456789 .,";
        return new string(str.ToCharArray().RemoveAll(c => !validChars.Contains(c)));
    }

    public static string RemoveChar(this string str, char characters)
    {
        return str.RemoveString(characters.ToString());
    }

    public static string RemoveChar(this string str, char[] allowedChars)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (allowedChars == null || allowedChars.Length == 0)
            return str;
        return new string(str.ToCharArray().RemoveAll(c => !allowedChars.Contains(c)));
    }

    [System.Reflection.Obfuscation()]
    public static string UncryptStringAES(this string str, byte[] key, byte[] vector)
    {
        string result = "";

#pragma warning disable S5542 // Encryption algorithms should be used with secure mode and padding scheme
        AesManaged aes = new()
        {
            Key = key,
        };
#pragma warning restore S5542 // Encryption algorithms should be used with secure mode and padding scheme
        if (vector.Length > 16)
            Array.Resize(ref vector, 16);
        aes.IV = vector;
        ICryptoTransform decrypter = aes.CreateDecryptor(key, vector);

        using (MemoryStream msDecrypt = new(Convert.FromBase64String(str)))
        {
            using CryptoStream csDecrypt = new(msDecrypt, decrypter, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            result = srDecrypt.ReadToEnd();
        }

        return result;
    }

    [System.Reflection.Obfuscation()]
    public static string EncryptStringAES(this string str, byte[] key, byte[] vector)
    {
        byte[] result = null;

#pragma warning disable S5542 // Encryption algorithms should be used with secure mode and padding scheme
        AesManaged aes = new()
        {
            Key = key,
        };
#pragma warning restore S5542 // Encryption algorithms should be used with secure mode and padding scheme
        if (vector.Length > 16)
            Array.Resize(ref vector, 16);
        aes.IV = vector;
        ICryptoTransform cryptor = aes.CreateEncryptor(key, vector);

        using (MemoryStream msEncrypt = new())
        {
            using CryptoStream csEncrypt = new(msEncrypt, cryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(str);
            }
            result = msEncrypt.ToArray();
        }

        return result != null ? Convert.ToBase64String(result) : "";
    }

    public static byte[] GetHashSHA256(this string chaine)
    {
        return chaine.GetHashSHA256(Encoding.UTF8);
    }

    public static byte[] GetHashSHA256(this string str, Encoding encoder)
    {
        SHA256Managed sha = new();
        return sha.ComputeHash(encoder.GetBytes(str));
    }

    public static string ToBase64(this string str)
    {
        return str.ToBase64(Encoding.UTF8);
    }

    public static string ToBase64(this string str, Encoding encoder)
    {
        return Convert.ToBase64String(encoder.GetBytes(str));
    }

    public static string FromBase64(this string str)
    {
        return str.FromBase64(Encoding.UTF8);
    }

    public static string FromBase64(this string str, Encoding encoder)
    {
        return encoder.GetString(Convert.FromBase64String(str));
    }

    public static string ToListOfString(this string[] list, string separator = ",")
    {
        StringBuilder ret = new();

        if (list.Length > 0)
            foreach (string item in list)
                ret.Append(separator + item);

        if (ret.ToString().Length > 0)
            return ret.ToString().Substring(1);
        else
            return ret.ToString();
    }

    public static bool Contains(this string str, string subString, StringComparison comparer)
    {
        if (!string.IsNullOrWhiteSpace(str) && !string.IsNullOrWhiteSpace(subString))
            return str.IndexOf(subString, comparer) >= 0;
        return false;
    }

    public static string Replace(this string str, string subString, string replacement, StringComparison comparer)
    {
        StringBuilder ret = new();

        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (str.Trim().Length == 0)
            return str;

        if (string.IsNullOrWhiteSpace(subString))
            throw new ArgumentNullException(nameof(subString));

        bool loop = true;
        while (loop)
        {
            loop = false;
            int find;
            if ((find = str.IndexOf(subString, 0, comparer)) >= 0)
            {
                ret.Clear();
                ret = new(str.Substring(0, find));
                ret.Append(replacement);
                ret.Append(str.Substring(find + subString.Length));
                str = ret.ToString();
                loop = true;
            }
        }

        return ret.Length == 0 ? str : ret.ToString();
    }

    public static string Replace(this string str, Dictionary<string, string> listToReplace)
    {
        string ret = str;
        listToReplace.Keys.ToList().ForEach(key => ret = ret.Replace(key, listToReplace[key]));
        return ret;
    }

    public static string Trim(this string str, string toRemove)
    {
        string ret = str;

        ret = ret.TrimStart(toRemove).TrimEnd(toRemove);

        return ret;
    }

    public static string TrimStart(this string str, string toRemove)
    {
        string ret = str;
        bool loop = true;

        while (loop)
        {
            loop = false;
            if (ret.StartsWith(toRemove))
            {
                loop = true;
                ret = ret.Substring(toRemove.Length);
            }
        }

        return ret;
    }

    public static string TrimEnd(this string str, string toRemove)
    {
        string ret = str;
        bool loop = true;

        while (loop)
        {
            loop = false;
            if (ret.EndsWith(toRemove))
            {
                loop = true;
                ret = ret.Substring(0, ret.Length - toRemove.Length);
            }
        }

        return ret;
    }

    public static string RemoveDuplicate(this string str, char toRemove)
    {
        string ret = str;
        while (ret.IndexOf(toRemove.ToString() + toRemove.ToString()) >= 0)
            ret = ret.Replace(toRemove.ToString() + toRemove.ToString(), toRemove.ToString());
        return ret;
    }

    public static string RemoveDuplicate(this string str, string toRemove)
    {
        string ret = str;
        while (ret.IndexOf(toRemove + toRemove) >= 0)
            ret = ret.Replace(toRemove + toRemove, toRemove);
        return ret;
    }

    public static SecureString ToSecureString(this string str)
    {
        SecureString retour = new();

        if (!string.IsNullOrEmpty(str))
            str.ToCharArray().ToList().ForEach(c => retour.AppendChar(c));

        return retour;
    }

    public static string MaxLength(this string str, int maxSize)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (str.Length > maxSize)
            return str.Substring(0, maxSize);
        else
            return str;
    }

    public static string RandomString()
    {
        return RandomString(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
    }

    public static string RandomString(int size)
    {
        return RandomString(size, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
    }

    public static string RandomString(int size, string authorizedChars)
    {
        return new string([.. Enumerable.Repeat(authorizedChars, size).Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)])]);
    }

    public static string[] Split(this string str, string separator)
    {
        return Split(str, separator, StringSplitOptions.None);
    }

    public static string[] Split(this string str, string separator, StringSplitOptions option)
    {
        List<string> ret = [];

        while (str.IndexOf(separator) >= 0)
        {
            ret.Add(str.Substring(0, str.IndexOf(separator)));
            str = str.Substring(str.IndexOf(separator) + separator.Length);
        }
        ret.Add(str);
        if (option == StringSplitOptions.RemoveEmptyEntries)
            ret.RemoveAll(ligne => string.IsNullOrWhiteSpace(ligne));

        return [.. ret];
    }

    public static string PreviousWord(this string str, ref int position)
    {
        return str.PreviousWord(ref position, true);
    }

    public static string PreviousWord(this string str, ref int position, bool ignoreCrLf)
    {
        string ret = "";
        if (string.IsNullOrWhiteSpace(str))
            return null;
        if (position > str.Length - 1)
            position = str.Length - 1;
        if (position == 0)
            return ret;
        while (str[position] != ' ' && position > 0 && ignoreCrLf && str[position] != '\r' && str[position] != '\n' || ret == "")
        {
            ret = str[position] + ret.Trim();
            position--;
        }
        return ret;
    }

    public static string NextWord(this string chaine, ref int position)
    {
        return chaine.NextWord(ref position, true);
    }

    public static string NextWord(this string chaine, ref int position, bool ignoreCrLf)
    {
        StringBuilder ret = new();
        if (string.IsNullOrWhiteSpace(chaine))
            return null;
        if (position > chaine.Length - 1)
            return ret.ToString();
        while (chaine[position] != ' ' && position < chaine.Length - 1 && ignoreCrLf && chaine[position] != '\r' && chaine[position] != '\n' || ret.Length == 0)
        {
            ret.Append(chaine[position].ToString().Trim());
            position++;
        }
        return ret.ToString();
    }

    public static string GenerateNextString(char[] listChars, string currentString, int maxChars)
    {
        if (string.IsNullOrEmpty(currentString) || currentString.Length < maxChars)
            return new string(listChars[0], maxChars);
        StringBuilder sb = new(currentString);
        if (sb.ToString() == new string(listChars[listChars.Length - 1], maxChars))
            return null;
        for (int i = sb.Length - 1; i >= 0; i--)
        {
            if (sb[i] != listChars[listChars.Length - 1])
            {
                sb[i] = listChars[Array.IndexOf(listChars, sb[i]) + 1];
                break;
            }
            else
                sb[i] = listChars[0];
        }
        return sb.ToString();
    }
}
