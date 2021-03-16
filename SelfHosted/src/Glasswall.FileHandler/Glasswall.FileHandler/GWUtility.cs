using Microsoft.SharePoint.Administration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace Glasswall.FileHandler
{
    static class GWUtility
    {
        private static SPDiagnosticsService logger = null;
        private static SPDiagnosticsCategory logCategory = null;
        private static SPDiagnosticsCategory logErrorCategory = null;
        private static bool IsLoaded = false;

        private static string _APIUrl = string.Empty;
        public static string APIUrl
        {
            get
            {
                _APIUrl = GetConfigValue(GWConstants.PROPS_REBUILD_API_URL);
                return _APIUrl;
            }
        }

        private static string _APIKey = string.Empty;
        public static string APIKey
        {
            get
            {
                _APIKey = GetConfigValue(GWConstants.PROPS_REBUILD_API_KEY);
                return _APIKey;
            }
        }

        static GWUtility()
        {
            logger = SPDiagnosticsService.Local;
            logCategory = new SPDiagnosticsCategory("GlasswallLog", TraceSeverity.Verbose, EventSeverity.Information);
            logErrorCategory = new SPDiagnosticsCategory("GlasswallLog", TraceSeverity.Unexpected, EventSeverity.Error);
        }

        public static void LoadConfiguration(string serverUrl)
        {
            if (!IsLoaded)
            {
                SPFarm farmObject = SPFarm.Local;
                if (farmObject.Properties != null && farmObject.Properties.Count > 0)
                {
                    if (farmObject.Properties.ContainsKey(GWConstants.PROPS_REBUILD_API_URL))
                    {
                        _APIUrl = Convert.ToString(farmObject.Properties[GWConstants.PROPS_REBUILD_API_URL]);
                        WriteLog($"Glasswall Rebuild Api Url found: {_APIUrl}.");
                    }
                    if (farmObject.Properties.ContainsKey(GWConstants.PROPS_REBUILD_API_KEY))
                    {
                        _APIKey = Convert.ToString(farmObject.Properties[GWConstants.PROPS_REBUILD_API_KEY]);
                        string strTrimmedKey = _APIKey.Trim().Length > 3 ? _APIKey.Trim().Substring(0, 3) : _APIKey.Trim();
                        WriteLog($"Glasswall Rebuild Api Key found: {strTrimmedKey}xxxxxxxxx.");
                    }
                }
                IsLoaded = true;
            }

        }
        public static void WriteLog(string message)
        {
            logger.WriteTrace(0, logCategory, TraceSeverity.Verbose, message);
        }
        public static void WriteErrorLog(string message)
        {
            logger.WriteTrace(0, logErrorCategory, TraceSeverity.Unexpected, message);
        }

        private static string GetConfigValue(string configKey)
        {
            SPFarm farmObject = SPFarm.Local;
            if (farmObject.Properties != null && farmObject.Properties.Count > 0)
            {
                if (farmObject.Properties.ContainsKey(configKey))
                {
                    return Convert.ToString(farmObject.Properties[configKey]);
                }
            }
            return string.Empty;
        }
    }
}
