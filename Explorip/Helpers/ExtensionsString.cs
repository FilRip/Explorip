namespace Explorip.Helpers;

/// <summary>
/// Classe contenant des extensions pour la classe String
/// </summary>
public static class ExtensionsString
{
    /// <summary>
    /// Return a string where remove all contiguous iteration of a specified character from a string<br/>
    /// Example : remove 3, 4, 5, ... same character, following, until there's no more iteration in string<br/>
    /// Example string : "0122234" with "2" as specified character return "01234"
    /// </summary>
    /// <param name="source">Source of string where we want to remove all contiguous specified character</param>
    /// <param name="character">Character that we want to remove all contiguous iteration</param>
    public static string RemoveDuplicate(this string source, char character)
    {
        string retour = source;
        while (retour.IndexOf(character.ToString() + character.ToString()) >= 0)
            retour = retour.Replace(character.ToString() + character.ToString(), character.ToString());
        return retour;
    }
}
