﻿using System.Runtime.InteropServices;
using System.Windows;

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de
// COM, affectez la valeur True à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

//Pour commencer à générer des applications localisables, définissez
//<UICulture>CultureUtiliséePourCoder</UICulture> dans votre fichier .csproj
//dans <PropertyGroup>.  Par exemple, si vous utilisez le français
//dans vos fichiers sources, définissez <UICulture> à fr-FR. Puis, supprimez les marques de commentaire de
//l'attribut NeutralResourceLanguage ci-dessous. Mettez à jour "fr-FR" dans
//la ligne ci-après pour qu'elle corresponde au paramètre UICulture du fichier projet.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //où se trouvent les dictionnaires de ressources spécifiques à un thème
                                     //(utilisé si une ressource est introuvable dans la page,
                                     // ou dictionnaires de ressources de l'application)
    ResourceDictionaryLocation.SourceAssembly //où se trouve le dictionnaire de ressources générique
                                              //(utilisé si une ressource est introuvable dans la page,
                                              // dans l'application ou dans l'un des dictionnaires de ressources spécifiques à un thème)
)]
