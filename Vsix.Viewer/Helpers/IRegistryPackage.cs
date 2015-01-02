namespace Vsix.Viewer.Helpers
{
    interface IRegistryPackage
    {
        /// <summary>
        /// [HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\12.0Exp_Config\Packages\{604ad610-5cf9-4bd5-8acc-f49810e2efd4}]
        /// </summary>
        string RegistryPath { get; set; }
        /// <summary> @="AnkhSVN - Subversion Support for Visual Studio" </summary>
        string Default { get; set; }
        /// <summary> "ProductName"="AnkhSVN" </summary>
        string ProductName { get; set; }
        /// <summary> "ProductVersion"="2.0" </summary>
        string ProductVersion { get; set; }
        /// <summary> "MinEdition"="Standard" </summary>
        string MinEdition { get; set; }
        /// <summary> "CodeBase"="C:\\Program Files (x86)\\AnkhSVN 2\\Ankh.Package.dll" </summary>
        string CodeBase { get; set; }
        /// <summary> 12.0 </summary>
        string VisualStudioVersion { get; set; }
        /// <summary> "Class"="Ankh.VSPackage.AnkhSvnPackage" </summary>
        string RegistryClass { get; set; }
        string RegistryGuid { get; set; }
    }
}
