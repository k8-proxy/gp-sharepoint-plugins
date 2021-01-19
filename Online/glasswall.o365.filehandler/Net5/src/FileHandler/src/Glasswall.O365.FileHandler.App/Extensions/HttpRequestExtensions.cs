using System.Collections.Specialized;
using Glasswall.O365.FileHandler.Models;
using Glasswall.O365.FileHandler.Utils;
using Microsoft.AspNetCore.Http;

namespace Glasswall.O365.FileHandler.App.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsFileHandlerActivationRequest(this HttpRequest request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;
            if (request.HasFormContentType)
            {
                // Get from current request's form data
                var nameValueCollection = new NameValueCollection();
                foreach (var key in request.Form.Keys)
                {
                    nameValueCollection.Add(key, request.Form[key]);
                }
                activationParameters = new FileHandlerActivationParameters(nameValueCollection);
                return true;
            }
            else
            {
                var persistedRequestData = CookieStorage.Load(request.Cookies);
                if (null != persistedRequestData)
                {
                    activationParameters = new FileHandlerActivationParameters(persistedRequestData);
                    return true;
                }
            }
            return false;
        }
    }
}
