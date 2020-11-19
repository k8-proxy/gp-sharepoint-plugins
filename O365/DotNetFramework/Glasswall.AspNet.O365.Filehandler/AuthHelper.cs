using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Glasswall.AspNet.O365.Filehandler.Utils;

namespace Glasswall.AspNet.O365.Filehandler
{

    public static class AuthHelper
    {
        public const string ObjectIdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private static ClientCredential clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);
        private const string AuthContextCacheKey = "authContext";
        
        public static async Task<string> GetUserAccessTokenSilentAsync(string resource, object cachedContext = null)
        {
            string signInUserId = GetUserId();
            if (!string.IsNullOrEmpty(signInUserId))
            {
                AuthenticationContext authContext = null;

                // Cache the authentication context in the session, so we don't have to reconstruct the cache for every call
                var session = System.Web.HttpContext.Current?.Session;
                if (session != null)
                {
                    authContext = session[AuthContextCacheKey] as AuthenticationContext;
                }

                if (authContext == null)
                {
                    authContext = new AuthenticationContext(SettingsHelper.Authority, false, new AzureTableTokenCache(signInUserId));

                    if (session != null)
                    {
                        session[AuthContextCacheKey] = authContext;
                    }
                }

                try
                {
                    var userCredential = new UserIdentifier(signInUserId, UserIdentifierType.UniqueId);
                    var authResult = await authContext.AcquireTokenSilentAsync(resource, clientCredential, userCredential);
                    return authResult.AccessToken;
                }
                catch (AdalSilentTokenAcquisitionException)
                {
                    
                }
            }
            return null;
        }
        
        public static string GetUserId()
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(ObjectIdentifierClaim);
            if (null != claim)
            {
                return claim.Value;
            }
            return null;
        }
    }
}