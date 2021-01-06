using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace Glasswall.FileHandler
{
    public class GlasswallRebuildClient
    {
        public byte[] RebuildFile(string base64)
        {
            var request = new JObject(){
                { "Base64",base64 }
            };
            string payload = JsonConvert.SerializeObject(request);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, GWUtility.APIUrl);
            requestMessage.Headers.TryAddWithoutValidation("x-api-key", GWUtility.APIKey);
            requestMessage.Headers.TryAddWithoutValidation( "accept", "*/*");
            requestMessage.Headers.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
            requestMessage.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
            requestMessage.Headers.TryAddWithoutValidation("content-type", "application/json");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
            requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                GWUtility.WriteLog($"GW rebuild api: {GWUtility.APIUrl}");
                var responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                GWUtility.WriteLog($"GW rebuild api response status: {responseMessage.StatusCode}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseContent = responseMessage.Content;
                    var regeneratedFile = responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                    var regeneratedFileBytes = Convert.FromBase64String(regeneratedFile);
                    return regeneratedFileBytes;
                }
                else
                {
                    GWUtility.WriteLog("Empty file return from REST API call");
                    return new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; // syntax for empty byte array
                }
            }

        }
    }
}
