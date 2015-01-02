using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using Ionic.Utils.Zip;
using Microsoft.Win32;
using Vsix.Common.Helpers;

namespace Vsix.Common
{
    public class CommonUtilities
    {
        private static string ILClientFolderName = "ILClient";
        private static string ILDeltaFolder = "VIA";

        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool wow64Process);

        public static String ConvertErrorCode(String code, String subcode)
        {
            String ErrorCode = String.Empty;
            String strConst = String.Empty;

            if (code != null && code != "")
            {
                code = code.Replace("-", "_");
                strConst = "IL_ARC_" + code + "_";
            }
            else
            {
                strConst = "IL_ARC_";
            }

            if (subcode != null && subcode != "")
            {
                subcode = subcode.Replace("-", "_");
                strConst = strConst + subcode;
            }

            return strConst;
        }

        public static void SerializeObject(string filename, Dictionary<string, string> dictFiles)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, dictFiles);
            stream.Close();
        }

        public static Dictionary<string, string> DeSerializeObject(string filename)
        {
            Dictionary<string, string> dictFiles = null;
            if (!String.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                Stream stream = File.Open(filename, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                dictFiles = (Dictionary<string, string>) bFormatter.Deserialize(stream);
                stream.Close();
            }
            return dictFiles;
        }

        public static string GetExchangeFilesDictionary(string exchangeId)
        {
            string retVal = "";

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folderPath = Path.Combine(folderPath, ILClientFolderName);
            folderPath = Path.Combine(folderPath, ILDeltaFolder);

            retVal = Path.Combine(folderPath, exchangeId + ".dat");

            return retVal;
        }



        public static string GetMyDocumentsFolderLocation()
        {
            string retVal = "";

            try
            {
                if (Environment.OSVersion.Version.Major <= 5)
                    retVal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                else
                    retVal = Environment.GetEnvironmentVariable("USERPROFILE");
            }
            catch (Exception)
            {
                retVal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            return retVal;
        }

        
        /// <summary>
        /// indicate whether Tour.dat file existed bedore application started
        /// </summary>
        public static bool TourDataFileExistedBeforeLaunch { get; set; }

        public static DateTime ConvertJavaMiliSecondToDateTime(long javaMS)
        {
            DateTime UTCBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dt = UTCBaseTime.Add(new TimeSpan(javaMS*TimeSpan.TicksPerMillisecond));
            return dt;
        }

        public static void RefreshOverlayIcons()
        {
            try
            {
                // Refresh only icons
                //0x8000000, 0x1000

                if (Environment.OSVersion.Version.Major <= 5)
                    SHChangeNotify(0x08000000, 0x0000 | 0x2000, IntPtr.Zero, IntPtr.Zero);
                else
                {
                    SHChangeNotify(0x00008000, 0x0003, IntPtr.Zero, IntPtr.Zero);
                    SHChangeNotify(0x08000000, 0x0000 | 0x2000, IntPtr.Zero, IntPtr.Zero);
                    SHChangeNotify(0x00000008, 0x0001 | 0x0003 | 0x2000 | 0x0005, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception){}
        }
        
        public static bool IsApplicationRunning(string appName)
        {
            return Process.GetProcessesByName(appName).Length > 0;
        }
        

        public static void CloseAllPendingApps(string appName)
        {
            try
            {
                Process[] processlist = Process.GetProcessesByName(appName);
                if (processlist.Length > 0)
                {
                    foreach (Process sendPackageProcess in processlist)
                    {
                        sendPackageProcess.CloseMainWindow();
                        sendPackageProcess.Kill();
                    }
                }
            }
            catch (Exception ex){}
        }

        private static string _productVersion;

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <returns></returns>
        public static string GetProductVersion()
        {
            if (String.IsNullOrEmpty(_productVersion))
            {
                var asm = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
                _productVersion = asm.GetName().Version.ToString();
            }
            return _productVersion;
        }

        /// <summary>
        /// Get comma separated list of Microsoft .NET Framework versions - read from registry
        /// </summary>
        /// <returns></returns>
        public static string GetAllFrameWorkVersions()
        {
            string frameWorkVersions = Environment.Version.ToString();
            const string msFramework = "Microsoft .NET Framework";
            const string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (var regKey = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                if (regKey != null)
                {
                    string[] subKeys = regKey.GetSubKeyNames();
                    if (subKeys.LongLength > 0)
                    {
                        foreach (var subKey in subKeys)
                        {
                            if (string.IsNullOrEmpty(subKey) || !subKey.Contains(msFramework))
                                continue;
                            frameWorkVersions += subKey.Replace(msFramework, ",");
                        }
                    }
                }
            }

            return frameWorkVersions;
        }

        /// <summary>
        /// get number of icon overlays, put names of overlays in log file
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfIconOverlays()
        {
            int i = 0;
            const string sreg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ShellIconOverlayIdentifiers";
            var regkey = Registry.LocalMachine.OpenSubKey(sreg);
            if (regkey == null)
                return 0;

            LogHelper.LogError(" listing icon overlays for " + Environment.MachineName);
            foreach (var r in regkey.GetSubKeyNames())
                LogHelper.LogError(string.Format("  {0,-3}. {1}", i++, r));

            return i;
        }

        public static string GetDefaultBrowserPath()
        {
            string defaultBrowserPath = null;
            var regkey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command", false);

            if (regkey != null)
                defaultBrowserPath = regkey.GetValue(null).ToString();
            else
            {
                // this is the case when 
                //   OperatingSystem os = Environment.OSVersion;
                //       if ((os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 6))
                regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\shell\Associations\UrlAssociations\http\UserChoice", false);
                if (regkey != null)
                    defaultBrowserPath = regkey.GetValue("Progid").ToString();
            }
            return defaultBrowserPath;
        }

        public static string GetBrowserNameVersion()
        {  
            string browserPath = GetDefaultBrowserPath();
            browserPath = browserPath.Split("\"'".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            return GetFileNameVersion(browserPath);
        }

        public static string GetFileNameVersion(string path)
        {
            if (!File.Exists(path))
                return string.Empty;

            // FileVersionInfo.FileVersion Property
            var fv = FileVersionInfo.GetVersionInfo(path);
            return fv.FileDescription + " " + fv.FileVersion;
        }

        private static bool IsOutlookInstalled()
        {
            Type requestType = Type.GetTypeFromProgID("Outlook.Application", false);
            if (requestType == null)
            {
                RegistryKey regkey = null;
                try
                {
                    regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Office", false);
                    if (regkey != null)
                    {
                        double version = 0.0;
                        string[] valueNames = regkey.GetSubKeyNames();
                        for (int i = 0; i < valueNames.Length; i++)
                        {
                            double temp = 0.0;
                            try
                            {
                                temp = Convert.ToDouble(valueNames[i], CultureInfo.CreateSpecificCulture("en-US").NumberFormat);
                            }
                            catch(Exception ex)
                            {
                                LogHelper.LogError(ex);
                            }
                            if (temp > version)
                                version = temp;
                        }
                        regkey.Close();
                        if (version != 0.0)
                            requestType = Type.GetTypeFromProgID(
                                "Outlook.Application." + version.ToString(CultureInfo.InvariantCulture).Replace(",", "."), false);
                    }
                }
                catch
                {
                    if (regkey != null)
                        regkey.Close();
                }
            }
            return (requestType != null);
        }

        public static string GetOfficeOutlookVersion()
        {
            string version = string.Empty;
            var regkey = Registry.ClassesRoot.OpenSubKey(@"Outlook.Application\CurVer", false);
            
            if (regkey != null)
            {
                version = regkey.GetValue(null).ToString();
                if (version.Contains('.'))
                    version = version.Split('.').Last();
                regkey.Close();
            }

            return GetOutlookFriendlyName(version);
        }

        public static string GetOutlookFriendlyName(string version)
        {
            version = version.ToLower();
            if (version.Contains("outlook.exe"))
            {
                // analyze string: "\"C:\\PROGRA~2\\MICROS~2\\Office14\\OUTLOOK.EXE\" -c IPM.Note /m \"%1\""
                var v = version.Split(@"/\".ToCharArray()).ToList().Where(x => x.Contains("office")).ToList().First();
                if (!string.IsNullOrEmpty(v)) version = v.Replace("office", "").Trim();
            }
            if (version.Equals("15")) return "Outlook 2013";
            if (version.Equals("14")) return "Outlook 2010";
            if (version.Equals("12")) return "Outlook 2007";
            if (version.Equals("11")) return "Outlook 2003";
            if (version.Equals("10")) return "Outlook 2002";
            if (version.Equals("9")) return "Outlook 2000";
            return version;
        }

        public static string GetOutlookVersion()
        {
            string version = string.Empty;
            try
            {
                string regOutlook = Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE",
                    "Path", null).ToString();

                if (!string.IsNullOrEmpty(regOutlook))
                {
                    var v = FileVersionInfo.GetVersionInfo(Path.Combine(regOutlook, "OUTLOOK.exe"));
                    version = v.FileVersion;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                //Console.WriteLine(ex.Message);
            }
            return version;
        }
        private enum WQL
        {
            Win32_OperatingSystem,  // Caption
            Win32_ComputerSystem,   // TotalPhysicalMemory
            Win32_Processor,        // NumberOfLogicalProcessors, MaxClockSpeed 
            // Win32_NetworkAdapter,
            // Win32_NetworkAdapterConfiguration 
            // Win32_NTDomain, // DcSiteName,DnsForestName,DomainName
            // Win32_DiskDrive
            // Win32_DiskPartition
            // Win32_DiskDriveToDiskPartition
        }

        private static string GetManagementObjectValue(WQL mo, string key)
        {
            var lst = new ManagementObjectSearcher("SELECT * FROM " + mo).Get().OfType<ManagementObject>().ToList();
            return "" + lst.First()[key];
        }

        /// <summary>
        /// get description of os of client machine
        /// </summary>
        /// <returns></returns>
        public static string GetOperatingSystemVersion()
        {
            string versionDetail = Environment.OSVersion.ToString() + (Is64BitProcess() ? ", 64-bit" : ", 32-bit");

            // user friendly name
            string osVersion = GetManagementObjectValue(WQL.Win32_OperatingSystem, "Caption");
            osVersion += string.Format("({0})", versionDetail);

            var memory = GetManagementObjectValue(WQL.Win32_ComputerSystem, "TotalPhysicalMemory");
            var gb = Convert.ToDouble(memory)/1024/1024/1024;
            osVersion += string.Format("; RAM {0:0.00}GB", gb);

            string freeSpace = GetHddFreeSpace().Aggregate("; Free Space ", (c, kvp) => c + string.Format("{0} {1}; ", kvp.Key, kvp.Value));
            osVersion += freeSpace;
            //var cpu = GetManagementObjectValue(WQL.Win32_Processor, "NumberOfLogicalProcessors");
            //osVersion += string.Format(", processors: {0}", cpu);

            var freq = Convert.ToDouble(GetManagementObjectValue(WQL.Win32_Processor, "MaxClockSpeed"));
            osVersion += string.Format("{0:0.00} Ghz", freq/1000);

            return osVersion;
        }

        /// <summary>
        /// check if app is running under admin priviliges
        /// </summary>
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity == null)
                return false;
            try
            {
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch(Exception ex)
            {
                LogHelper.LogError(ex);
                return false;
            }
        }
        public static Dictionary<string, string> GetHddFreeSpace()
        {
            var dict =new Dictionary<string, string>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                // exclude floppy, cdrom, dvd, not ready drives
                try
                {
                    dict.Add(drive.Name, "" + drive.AvailableFreeSpace/1024/1024/1024 + "GB");
                }
                catch (Exception)
                {
                }
            }
            return dict;
        }

        /// <summary>
        /// get default mail client and its version, similar to Outlook.14
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultMailToClient()
        {
            // @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice"
            // Progid   key  Outlook.URL.mailto.14
            string defaultMailClient = string.Empty;
            var regkey =
                Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", false);
            if (regkey != null)
            {
                defaultMailClient = regkey.GetValue("Progid").ToString();
                regkey.Close();
            }

            if (!string.IsNullOrEmpty(defaultMailClient) && defaultMailClient.ToLower().Contains("mailto"))
                defaultMailClient = defaultMailClient.ToLower().Replace(".url", "").Replace(".mailto", "");
            else
            {
                // some machines have default mail client set up in 
                // HKEY_LOCAL_MACHINE\SOFTWARE\Classes\mailto\shell\open\command
                regkey = Registry.LocalMachine.OpenSubKey(@"Software\Classes\mailto\shell\open\command", false);
                if (regkey != null)
                {
                    defaultMailClient = regkey.GetValue(null).ToString();
                    // expected line 
                    // "\"C:\\PROGRA~2\\MICROS~2\\Office14\\OUTLOOK.EXE\" -c IPM.Note /m \"%1\""
                    regkey.Close();
                }
            }

            defaultMailClient = defaultMailClient.ToLower();
            if (defaultMailClient.Contains(".exe") && !defaultMailClient.Contains("outlook"))
            {
                var v = defaultMailClient.Split(@"/ \""".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                         .ToList()
                                         .Where(x => x.Contains(".exe"))
                                         .ToList()
                                         .First();
                defaultMailClient = v;
            }
            else if (defaultMailClient.Contains("outlook"))
                defaultMailClient = GetOutlookFriendlyName(defaultMailClient);

            return defaultMailClient;
        }

        public static string GetExecutedAssemblyVersion()
        {
            var prodVer = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}", prodVer.Major, prodVer.Minor, prodVer.Build);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailTo">vsix.bndrv@gmail.com</param>
        /// <param name="logfilesPath"></param>
        /// <param name="applicationName"></param>
        /// <param name="mailContent"></param>
        /// <param name="mailSubject"></param>
        public static void ShowSendLogFilesPopup(string emailTo, string logfilesPath, string applicationName, string mailContent, string mailSubject = null)
        {
            string supportEmailSubject =string.IsNullOrEmpty(mailSubject)?
                string.Format("{0} Logs as of {1}", applicationName, DateTime.Now.ToString("G")) : mailSubject;

            string archive = CompressFolderRecursive(logfilesPath);

            //SendMailProcess(supportEmailAddress, null, null, mailSubject, mailContent, archive);

            try
            {
                // defaultMailClient
                var mapi = new MAPI();
                mapi.AddRecipientTo(emailTo);
                if (!String.IsNullOrEmpty(archive) && File.Exists(archive))
                    mapi.AddAttachment(archive);

                // res = 0 - message sent, res = 1 - user abort, others - error while send
                // use mapi API from [DllImport("MAPI32.DLL")]
                int res = mapi.SendMailPopup(supportEmailSubject, mailContent);

                if (res > 2)
                {
                    System.Windows.Forms.MessageBox.Show(
                        mapi.GetLastError(),
                        "ErrReport - No Mail Client Caption",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //// if previous attempt failed, try to run it from process... but without attachment
                    //ShowSendLogFilesPopupProcess(supportEmailAddress, null, null, supportEmailSubject, mailContent, archive);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        private static string EscapeString(string s)
        {
            return (s == null) ? null : Uri.EscapeDataString(s);
        }

        /// <summary>
        /// send mail via default mail client via process
        /// </summary>
        /// <param name="to">To email addresses delimeted by a semi-colon.</param>
        /// <param name="cc">The Cc email addresses delimeted by a semi-colon.</param>
        /// <param name="bcc">The Bcc email addresses delimeted by a semi-colon.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        /// <param name="attachment">The attachment file path. Does not work for Outlook 2010.</param>
        public static void ShowSendLogFilesPopupProcess(string to, string cc, string bcc, string subject, string body, string attachment)
        {
            try
            {
                if (string.IsNullOrEmpty(to)) to = "some@here.net";
                if (string.IsNullOrEmpty(subject)) subject = "...";

                string s = Uri.UriSchemeMailto + ':' + EscapeString(to) + '?';
                s += "subject=" + EscapeString(subject);
                if (!string.IsNullOrEmpty(attachment) && File.Exists(attachment))
                {
                    // this works only for Outlook 2003
                    s += "&attachment=" + EscapeString(attachment);

                    // mention specifically about attaching archive, auto attach does not work for Outlook 2010:
                    body = body.Replace("\n> A", "\n> O. Please attach file: " + attachment + "\n> A");
                }
                if (!string.IsNullOrEmpty(body)) s += "&body=" + EscapeString(body);
                if (!string.IsNullOrEmpty(cc)) s += "&cc=" + EscapeString(cc);
                if (!string.IsNullOrEmpty(bcc)) s += "&bcc=" + EscapeString(bcc);

                var process = new Process {StartInfo = new ProcessStartInfo(s)};
                process.StartInfo.Verb = "runas";
                process.Start();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        public static string CompressFolderRecursive(string inputDir)
        {
            var dir = new DirectoryInfo(inputDir);
            string uniqueTempDir = GetTempDirectory(Path.GetTempPath());
            var copiedDirectory = Directory.CreateDirectory(Path.Combine(uniqueTempDir, dir.Name));
            CopyDirectory(dir.FullName, copiedDirectory.FullName);
            string compressedFilePath = Path.Combine(uniqueTempDir, "LogFiles.zip");
            var f = new ZipFile(compressedFilePath);
            CompressDirectory(ref f, copiedDirectory.FullName, dir.Name);
            f.Save();
            Directory.Delete(copiedDirectory.FullName, true);
            return File.Exists(compressedFilePath) ? compressedFilePath : String.Empty;
        }

        private static void CompressDirectory(ref ZipFile zipFile, string inputDir, string archiveDirName)
        {
            var dir = new DirectoryInfo(inputDir);
            if (!dir.Exists || zipFile == null) return;

            zipFile.AddDirectoryByName(archiveDirName);

            foreach (var file in dir.GetFiles())
            {
                if (file != null && file.Exists)
                    zipFile.AddFile(file.FullName, archiveDirName);
            }

            foreach (var subDir in dir.GetDirectories())
                CompressDirectory(ref zipFile, subDir.FullName, Path.Combine(archiveDirName, subDir.Name));
        }

        public static void CopyDirectory(string src, string dst)
        {
            if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);
            foreach (string file in Directory.GetFileSystemEntries(src))
            {
                // Sub directories
                if (Directory.Exists(file))
                    CopyDirectory(file, dst + Path.GetFileName(file));
                else
                    File.Copy(file, dst + Path.GetFileName(file), true);
            }
        }

        private static string GetTempDirectory(string sourcePath)
        {
            string path = Path.Combine(Path.GetFullPath(sourcePath), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }


        /// <summary>
        /// Checks if running platform is 64bit platform
        /// </summary>
        /// <returns>True for 64bit Platform / False for 32bit Platform</returns>
        public static bool Is64BitProcess()
        {
            bool is64BitProcess = (IntPtr.Size == 8);
            bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();
            return is64BitOperatingSystem;
        }

        /// <summary>
        /// Checks if current process is Wow64Process
        /// </summary>
        /// <returns>True for 64bit Platform / False for 32bit Platform</returns>
        private static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal)) return false;
                    return retVal;
                }
            }
            else return false;
        }

        /// <summary>
        /// Encode to base 64 string
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }
}