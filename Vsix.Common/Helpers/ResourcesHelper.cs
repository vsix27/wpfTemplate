using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Vsix.Common.Properties;

namespace Vsix.Common.Helpers
{
    public sealed class ResourcesHelper
    {

        private ResourcesHelper()
        {
            CultureInfo appCulture = GetAppCulture();
            Resources.Culture = appCulture;
        }

        public string DisplayName
        {
            get { return GetAppCulture().DisplayName; }
        }

        private  CultureInfo _appCulture;

        public  void SetAppCulture(string localeCode)
        {
            Thread.CurrentThread.CurrentCulture.ClearCachedData();
            Thread.CurrentThread.CurrentUICulture.ClearCachedData();

            _appCulture = new CultureInfo(localeCode);

            Thread.CurrentThread.CurrentCulture = _appCulture;
            Thread.CurrentThread.CurrentUICulture = _appCulture;
        }

        public  CultureInfo GetAppCulture()
        {
            try
            {
                if (_appCulture == null)
                {
                    LogHelper.LogEnter();
                    string localeCode = ReadLocale();
                    if (string.IsNullOrEmpty(localeCode)) localeCode = "en-US";
                    _appCulture = new CultureInfo(localeCode);
                    LogHelper.LogExit(_appCulture);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                _appCulture = new CultureInfo("en-US");
            }
            return _appCulture;
        }

        public static string ReadLocale()
        {
            LogHelper.LogEnter();
            string retVal = "en-US";

            try
            {
                string localeLocationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "locale.dat");

                if (File.Exists(localeLocationPath))
                {
                    var textReader = new StreamReader(localeLocationPath);
                    string line = textReader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        retVal = line;
                    textReader.Close();
                }
                else return retVal;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            LogHelper.LogExit(retVal);
            return retVal;
        }

        private static readonly ResourcesHelper _instance = new ResourcesHelper();

        public static ResourcesHelper Instance
        {
            get { return _instance; }
        }

        public string GetString(string resourceKey)
        {
            try
            {
                return GetResourceObject(resourceKey) as String;
            }
            catch (InvalidCastException ice)
            {
                LogHelper.LogError("GetString - " + resourceKey, ice);
            }
            return null;
        }

        private object GetResourceObject(string resourceKey)
        {
            if (resourceKey == null) return null;
            return Resources.ResourceManager.GetObject(resourceKey, GetAppCulture());
        }
    }
}
