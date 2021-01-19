using System;
using System.Web;

namespace Glasswall.AspNet.O365.Filehandler
{
    public static class ActionHelpers
    {
        public static string ParseBaseUrl(string oneDriveApiSourceUrl)
        {
            var trimPoint = oneDriveApiSourceUrl.IndexOf("/drive");
            return oneDriveApiSourceUrl.Substring(0, trimPoint);
        }

        public static string BuildApiUrl(string baseUrl, string driveId, string itemId, string extra = "")
        {
            return $"{baseUrl}/drives/{driveId}/items/{itemId}/{extra}";
        }
    }
}
