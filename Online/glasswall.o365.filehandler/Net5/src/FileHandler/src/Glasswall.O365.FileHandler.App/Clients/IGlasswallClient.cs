using System.Net.Http;
using System.Threading.Tasks;
using Glasswall.O365.FileHandler.App.Models;
using Refit;

namespace Glasswall.O365.FileHandler.App.Clients
{
    public interface IGlasswallClient
    {
        [Post("/Rebuild/base64")]
        [Headers("Accept:*/*",
            "accept-encoding: gzip, deflate, br",
            "accept-language:en-US,en;q=0.9",
            "content-type", "application/json",
            "sec-fetch-dest", "empty",
            "sec-fetch-mode:cors",
            "sec-fetch-site:cross-site")]
        Task<HttpResponseMessage> RebuildFile([Body] RebuildPayload rebuildPayload);
    }
}
