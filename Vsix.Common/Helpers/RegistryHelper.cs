using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using log4net;
using Microsoft.Win32;

namespace Vsix.Common.Helpers
{
    
    /* purpose of this class is to change registry so application will start with windows for specific user
     * it is also possible to do with proper shortcuts (place shortcut under Programs/Startup folder)
     * 
     * this class modifies registry and application should have elevated privileges when uses such registry calls.
     * 
     * 1.
     * elevated privileges - obtained with manifest file:
     * in project 
     * mail app.csproj
     * add app.manifest file
     * and in line 
     *          <requestedExecutionLevel level="asInvoker" uiAccess="false" />
     * change to asInvoker to requireAdministrator
     *          <requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
     * if such privileges no needed then either remove manifest file from project or use level="asInvoker"
     * 
     * 
     * 2. to make installer to start application when windows starts 
     * startup registry was changed as well:
     * in installer project hit 
     * Registry Editor, create tree under HKEY_CURRENT_USER:
     *      Software
     *          Microsoft    --  this value has default: [Manufacturer] and has no children so the rest should be created
     *              Windows
     *                  CurrentVersion
     *                      Run
     *                      

     */
    /*
        .NET app compiled for "x86":
            Always 32-bit
            On 32-bit platforms, accesses 32-bit registry
            On 64-bit platforms, accesses 32-bit registry (inside Wow6432Node)
                64-bit registry inaccessible (without doing something weird)
     * 
        .NET app compiled for "x64":
            Always 64 bit
            On 32-bit platforms, won't run
            On 64-bit platforms, accesses 64-bit registry (not inside Wow6432Node)
                If you want to get to the 32-bit registry, you need to do something weird
     * 
        .NET app compiled for "AnyCpu"
            Either 32 or 64 bit depending on platform
            On 32-bit platforms, accesses 32-bit registry
            On 64-bit platforms, accesses 64-bit registry (not inside Wow6432Node)
                If you want to get to the 32-bit registry, you need to do something weird   
     * 
     * "Something weird" technically requires passing a special flag to RegOpenKeyEx to obtain the alternate registry view. 
     */

    /// <summary>
    /// Registry helper class to add app to startup or remove from there, to read registry keys
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary> \SOFTWARE\Microsoft\Windows\CurrentVersion\Run </summary>
        private const string SOFTWARE_RUN = @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string HKLM_RUN = @"HKEY_LOCAL_MACHINE" + SOFTWARE_RUN;
        public const string HKCU_RUN = @"HKEY_CURRENT_USER" + SOFTWARE_RUN;

        /// <summary> \SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run </summary>
        private const string SOFTWARE_RUN64 = @"\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";
        public const string HKLM_RUN64 = @"HKEY_LOCAL_MACHINE" + SOFTWARE_RUN64;
        public const string HKCU_RUN64 = @"HKEY_CURRENT_USER" + SOFTWARE_RUN64;

        /// <summary> HKEY_CURRENT_USER\Software\Microsoft\VisualStudio </summary>
        public const string HKCU_VS = @"HKEY_CURRENT_USER\Software\Microsoft\VisualStudio";

        public const string NOT_AN_ADMINISTRATOR = "Application is not running with Administrator privileges";

        private static readonly ILog logger = LogManager.GetLogger(typeof(RegistryHelper));

        public static bool Is64Bit()
        {
            // 8 for 64 bit machine, 4 for 32 bit machine; do not rely on IsWow64Process from DllImport("kernel32.dll"...
            return (IntPtr.Size == 8); 
        }

        private static RegistryKey SoftwareKeyHKLM
        {
            get
            {
                // in .net 4
                // see also http://stackoverflow.com/questions/7202752/cannot-write-to-the-registry-key
                //  if (Environment.Is64BitOperatingSystem)
                //     return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE");
                return Registry.LocalMachine.OpenSubKey("SOFTWARE");
            }
        }

        private static RegistryKey SoftwareKeyHKCU
        {
            get { return Registry.CurrentUser.OpenSubKey("SOFTWARE"); }
        }

        public static void AddToHKLMStartUp(string appName, string appPath)
        {
            if (string.IsNullOrEmpty(Read(HKLM_RUN, appName)))
                Write(HKLM_RUN, appName, appPath);
        }

        public static void AddToHKCUStartUp(string appName, string appPath)
        {
            if (string.IsNullOrEmpty(Read(HKLM_RUN, appName)))
                Write(HKCU_RUN, appName, appPath);
        }

        public static string GetHKLMStartUp(string appName)
        {
            return Read(HKLM_RUN, appName);
        }

        public static string GetHKCUStartUp(string appName)
        {
            return Read(HKCU_RUN, appName);
        }
        public static string[] GetHKCUStartUps()
        {
            RegistryKey rk = GetRegistryPath(HKCU_RUN);

            if (rk == null)
                return null;

            return rk.GetValueNames();
        }


        // remove from comp startup
        public static void RemoveFromStartUp(string appName)
        {
            if (!string.IsNullOrEmpty(Read(HKLM_RUN, appName)))
                DeleteKey(HKLM_RUN, appName);

            if (!string.IsNullOrEmpty(Read(HKCU_RUN, appName)))
                DeleteKey(HKCU_RUN, appName);
        }

        /// <summary>
        /// read registry, path can start from HKEY_LOCAL_MACHINE or HKLM; HKEY_CURRENT_USER or HKCU
        /// </summary>
        /// <param name="registryPath"></param>
        /// <param name="createIfMissing"></param>
        /// <returns></returns>
        public static RegistryKey GetRegistryPath(string registryPath, bool createIfMissing = false)
        {
            if (string.IsNullOrEmpty(registryPath))
                return null;

            string[] parts = registryPath.Split(@"/\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return null;

            RegistryKey rk;

            string root = parts[0].ToUpper();

            if (root.Equals("HKEY_LOCAL_MACHINE") || root.Equals("HKLM"))
                rk = Registry.LocalMachine;
            else if (root.Equals("HKEY_CURRENT_USER") || root.Equals("HKCU"))
                rk = Registry.CurrentUser;
            else
                throw new Exception("invalid registry entry key provided: " + root);

            for (int k = 1; k < parts.Length; k++)
            {
                if (createIfMissing)
                {
                    CheckIfAppHasAdminRights();

                    var subrk = rk.OpenSubKey(parts[k], true);
                    if (subrk == null)
                    {
                        rk.CreateSubKey(parts[k], RegistryKeyPermissionCheck.ReadWriteSubTree);
                        subrk = rk.OpenSubKey(parts[k], true);
                    }
                    rk = subrk;
                }
                else
                {
                    rk = rk.OpenSubKey(parts[k], true);
                    if (rk == null)
                        return null;
                }
            }

            return rk;
        }

        public static string Read(string registryPath, string keyName)
        {
            RegistryKey rk = GetRegistryPath(registryPath);

            if (rk == null)
                return null;

            var o = rk.GetValue(keyName);

            return o != null ? o.ToString() : null;
        }

        public static bool Write(string registryPath, string keyName, object val)
        {
            try
            {
                CheckIfAppHasAdminRights();
                
                RegistryKey rk = GetRegistryPath(registryPath, true);

                if (rk == null)
                    return false;

                // Save the value
                rk.SetValue(keyName, val);

                return true;
            }
            catch (Exception ex)
            {
                //Console.Write(ex);
                logger.Error(ex);
                return false;
            }
        }

        public static Dictionary<string, string> ReadKeyList(string registryPath)
        {
            RegistryKey rk = GetRegistryPath(registryPath);

            if (rk == null)
                return null;

            var dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (string s in rk.GetValueNames())
                dict.Add(s, rk.GetValue(s).ToString());

            return dict;
        }

        public static bool DeleteKey(string registryPath, string keyName)
        {
            try
            {
                CheckIfAppHasAdminRights();

                RegistryKey rk = GetRegistryPath(registryPath);

                if (rk == null)
                    return true;

                rk.DeleteValue(keyName);

                return (rk.GetValue(keyName) == null);
            }
            catch (Exception ex)
            {
                //Console.Write(ex);
                logger.Error(ex);
                return false;
            }
        }

        public static void CheckIfAppHasAdminRights()
        {
            if (!IsUserAdministrator)
                throw new Exception(NOT_AN_ADMINISTRATOR);
        }

        public static bool IsUserAdministrator
        {
            get
            {
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

                // Get the SID of the admin group on the local machine.
                var localAdminGroupSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                //Then you can check the Groups property on the WindowsIdentity of the user to see if that user is a member of the local admin group, like so:
                bool isLocalAdmin = windowsIdentity.Groups.Select(
                    g => (SecurityIdentifier) g.Translate(typeof (SecurityIdentifier)))
                                                   .Any(s => s == localAdminGroupSid);
                return isLocalAdmin;
            }
        }

        /// <summary>Opens RegEdit to the provided key
        /// <para><example>@"Computer\HKEY_CURRENT_USER\Software\MyCompanyName\MyProgramName\"</example></para>
        /// </summary>
        /// <param name="fullKeyPath"></param>
        public static void OpenToKey(string fullKeyPath)
        {
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Applets\Regedit", true);
            if (rKey == null) return;
            rKey.SetValue("LastKey", fullKeyPath);
            Process.Start("regedit.exe");
        }
    }
}
