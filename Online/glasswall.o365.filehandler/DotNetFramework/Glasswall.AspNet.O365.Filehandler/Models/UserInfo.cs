using System.Threading.Tasks;

namespace Glasswall.AspNet.O365.Filehandler.Directory
{

    public class UserInfo
    {
        public static async Task<UserInfo> GetUserInfoAsync(string graphBaseUrl, string userObjectId, string accessToken)
        {
            if (!string.IsNullOrEmpty(userObjectId))
            {
                return await HttpHelper.Default.GetMetadataForUrlAsync<UserInfo>($"{graphBaseUrl}/v1.0/users/{userObjectId}?$select=displayName,mysite", accessToken);
            }

            return null;
        }

        public string DisplayName { get; set; }
        public string MySite { get; set; }

    }
}
