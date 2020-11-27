using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Les informations générales relatives à un assembly dépendent de
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyTitle("MethodRedirect")]
[assembly: AssemblyDescription("Extension to redirect a method to or swap it with another method.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("NSPX Software")]
[assembly: AssemblyProduct("MethodRedirect")]
[assembly: AssemblyCopyright("Copyright © NSPX Software 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Allow unit tests project to access internal methods
[assembly: InternalsVisibleTo("MethodRedirect_UT")]

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

// Le GUID suivant est pour l'ID de la typelib si ce projet est exposé à COM
[assembly: Guid("2a84dc12-471e-492f-b390-df3adb452e28")]

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de build et de révision par défaut
// en utilisant '*', comme indiqué ci-dessous :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
