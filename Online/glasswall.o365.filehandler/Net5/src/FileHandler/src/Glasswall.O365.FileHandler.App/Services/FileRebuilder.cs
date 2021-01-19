using System;
using System.Threading.Tasks;
using Glasswall.O365.FileHandler.App.Models;
using Glasswall.O365.FileHandler.App.Clients;

namespace Glasswall.O365.FileHandler.App.Services
{
    public class FileRebuilder : IFileRebuilder
    {
        private readonly IGlasswallClient _glasswallClient;

        public FileRebuilder(IGlasswallClient glasswallClient)
        {
            _glasswallClient = glasswallClient;
        }

        public async Task<byte[]> Rebuild(string base64String)
        {
            var rebuildPayload = new RebuildPayload
            {
                Base64 = base64String
            };
            
            //string payload = JsonSerializer.Serialize(rebuildPayload);
            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, _glasswallSettings.ApiUrl);
            //requestMessage.Headers.TryAddWithoutValidation("x-api-key", _glasswallSettings.ApiKey);
            //requestMessage.Headers.TryAddWithoutValidation("Accept", "*/*");
            //requestMessage.Headers.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
            //requestMessage.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
            //requestMessage.Headers.TryAddWithoutValidation("content-type", "application/json");
            //requestMessage.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
            //requestMessage.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
            //requestMessage.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
            //requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var responseMessage = await _glasswallClient.RebuildFile(rebuildPayload);
            responseMessage.EnsureSuccessStatusCode();
            var regeneratedFile = await responseMessage.Content.ReadAsStringAsync();
            var regeneratedFileBytes = Convert.FromBase64String(regeneratedFile);
            return regeneratedFileBytes;
        }
    }
}
