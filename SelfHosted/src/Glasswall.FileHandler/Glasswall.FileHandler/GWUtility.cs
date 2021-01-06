using Microsoft.SharePoint.Administration;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace Glasswall.FileHandler
{
    static class GWUtility
    {
        private static string gwRebuildApi = string.Empty;
        private static string gwRebuildApiKey = string.Empty;
        private static SPDiagnosticsService logger = null;
        private static SPDiagnosticsCategory logCategory = null;
        private static SPDiagnosticsCategory logErrorCategory = null;
        private static bool IsLoaded = false;
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
                var jsonUrl = serverUrl + "/_layouts/15/Glasswall.FileHandler/gwkey.txt";
                JObject jsonConfig = JObject.Parse(GetFileContent(jsonUrl));
                foreach (JProperty prop in jsonConfig.Properties())
                {
                    if (prop.Name == GWConstants.RebuildApiKeySettingsName)
                    {
                        gwRebuildApiKey = prop.Value.ToString();
                        WriteLog($"Property Key GwRebuildApiKey Found: {gwRebuildApiKey.Substring(0,3)}xxxxxxx");
                    }
                    if (prop.Name == GWConstants.RebuildApiUrlSettingsName)
                    {
                        gwRebuildApi = prop.Value.ToString();
                        WriteLog($"Property Key GwRebuildApi Found:{gwRebuildApi}");
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
        private static string GetFileContent(string path)
        {
            WebRequest request = WebRequest.Create(path);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public static string APIUrl { get { return gwRebuildApi; } }
        public static string APIKey { get { return gwRebuildApiKey; } }
    }
}
