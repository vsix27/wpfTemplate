using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Vsix.Common.Helpers;

namespace Vsix.Viewer.Helpers
{
    public class RegistryPackage : IRegistryPackage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rk"></param>
        /// <param name="n"></param>
        public RegistryPackage(Microsoft.Win32.RegistryKey rk)
        {
            if (rk == null) return;
           
            ProductName = "" + rk.GetValue("ProductName");
            ProductVersion = "" + rk.GetValue("ProductVersion");
            MinEdition = "" + rk.GetValue("MinEdition");

            // skip init to save time
            if (!IsPackage) return;

            Default = "" + rk.GetValue(null);
            CodeBase = "" + rk.GetValue("CodeBase");
            RegistryPath = rk.Name;
            RegistryClass = "" + rk.GetValue("Class");
            var rways = RegistryPath.Split("\\[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            RegistryGuid = rways.Last();
            if (rways.Length > 5)
                VisualStudioVersion = rways[4].Split('.')[0];
        }



        /// <summary>
        /// [works for windows 8] returns list of strings with registry paths similar to 
        /// HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\10.0Exp_Config\Packages
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegistryPackagesPath()
        {
            //var list = RegistryHelper.GetVisualStudioExtensionPath();

            // works for windows 8
            var sKeys = RegistryHelper.GetRegistryPath(RegistryHelper.HKCU_VS).GetSubKeyNames().ToList();
            Func<string, string> addPackage = (c) => RegistryHelper.HKCU_VS + "\\" + c + "\\Packages";
            var sKeysPacks = sKeys.Where(c => c.EndsWith("Exp_Config", StringComparison.OrdinalIgnoreCase)).Select(c => addPackage(c)).ToList();
            return sKeysPacks;
        }

        private bool IsPackage
        {
            get
            {
                return !string.IsNullOrEmpty(ProductName) &&
                       !string.IsNullOrEmpty(ProductVersion) &&
                       !string.IsNullOrEmpty(MinEdition);
            }
        }

        #region properties

        public int N { get; set; }

        /// <summary>
        /// [HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\12.0Exp_Config\Packages\{604ad610-5cf9-4bd5-8acc-f49810e2efd4}]
        /// </summary>
        public string RegistryPath { get; set; }

        /// <summary> @="AnkhSVN - Subversion Support for Visual Studio" </summary>
        public string Default { get; set; }

        /// <summary> "ProductName"="AnkhSVN" </summary>
        public string ProductName { get; set; }

        /// <summary> "ProductVersion"="2.0" </summary>
        public string ProductVersion { get; set; }

        /// <summary> "MinEdition"="Standard" </summary>
        public string MinEdition { get; set; }

        /// <summary> "CodeBase"="C:\\Program Files (x86)\\AnkhSVN 2\\Ankh.Package.dll" </summary>
        public string CodeBase { get; set; }

        /// <summary> 12.0 </summary>
        public string VisualStudioVersion { get; set; }

        /// <summary> "Class"="Ankh.VSPackage.AnkhSvnPackage" </summary>
        public string RegistryClass { get; set; }
        public string RegistryGuid { get; set; }

        #endregion


        /// <summary> values of all properties with values </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // return ToStrings().Aggregate(string.Empty, (current, c) => current + (c + Environment.NewLine));
            return ToStringPairs().Aggregate(string.Empty, (current, c) => current + (c.Key + " " + c.Value + Environment.NewLine));
        }

        /// <summary> list of all properties with values </summary>
        /// <returns></returns>
        public List<string> ToStrings()
        {
            return ToStringPairs().Select(kvp => kvp.Key + " " + kvp.Value).ToList();
        }

        public Dictionary<string, string> ToStringPairs(bool visibleOnly = false)
        {
            var dict = new Dictionary<string, string>
            {
                {"ProductName", ProductName},
                {"ProductVersion", ProductVersion},
                {"MinEdition", MinEdition},
                {"CodeBase", CodeBase},
                {"RegistryGuid", RegistryGuid},
                {"RegistryPath", RegistryPath},
            };
            if (!visibleOnly)
            {
                dict.Add("Default", Default);
                dict.Add("VisualStudioVersion", VisualStudioVersion);
                dict.Add("RegistryClass", RegistryClass);
            }

            return dict;
        }
    }
}
