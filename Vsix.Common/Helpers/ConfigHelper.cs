using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vsix.Common.Helpers
{
    public class ConfigHelper
    {
        public static string GetSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}
