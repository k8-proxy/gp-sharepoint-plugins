using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Glasswall.O365.FileHandler.Utils
{
    public class CookieStorage
    {
        private const string CookieName = "FileHandlerActivationParameters";


        public static NameValueCollection Load(IRequestCookieCollection cookies)
        {
            var cookie = cookies[CookieName];
            if (cookie != null)
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(cookie);
                NameValueCollection collection = new NameValueCollection();
                foreach(var key in dict.Keys)
                {
                    collection.Add(key, dict[key]);
                }
                return collection;
            }
            else
            {
                return null;
            }
        }

        public static NameValueCollection Load(HttpRequest request)
        {
            return Load(request.Cookies);
        }

        public static void Save(NameValueCollection collection, HttpResponse response)
        {
            Dictionary<string,string> pairs = new Dictionary<string, string>();
            foreach(string key in collection.AllKeys)
            {
                pairs.Add(key, collection[key]);
            }

            var cookie = new CookieOptions(){
                Expires = DateTime.Now.AddMinutes(10)
            };
            response.Cookies.Append(CookieName,JsonSerializer.Serialize(pairs),cookie);
        }

        public static void Save(IFormCollection formCollection, HttpResponse response)
        {
            Dictionary<string,string> pairs = new Dictionary<string, string>();
            foreach(var key in formCollection.Keys)
            {
                pairs.Add(key, formCollection[key]);
            }
            var cookie = new CookieOptions(){
                Expires = DateTime.Now.AddMinutes(10)
            };
            response.Cookies.Append(CookieName,JsonSerializer.Serialize(pairs),cookie);
        }

        public static void Clear(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies.Keys.Contains(CookieName))
            {
                var cookie = new CookieOptions(){
                    Expires = DateTime.Now.AddDays(-1)
                };
                var cookieValue = request.Cookies[CookieName];
                response.Cookies.Append(CookieName,cookieValue,cookie);
            }
        }
    }
}
