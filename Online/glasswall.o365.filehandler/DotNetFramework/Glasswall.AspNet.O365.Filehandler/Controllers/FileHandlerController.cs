using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Glasswall.AspNet.O365.Filehandler.Models;
using Microsoft.Graph;

namespace Glasswall.AspNet.O365.Filehandler.Controllers
{

    [Authorize]
    public class FileHandlerController : Controller
    {

        #region Methods registered in file handler manifest
        /// <summary>
        /// Custom action to rebuild the file using Glasswall rebuild engine
        /// </summary>
        public async Task<FileStreamResult> Download()
        {
            var input = GetActivationParameters(Request);
            var fileUrl = input.ItemUrls.First();
            var accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(input.ResourceId);
            
            //Get file content
            var sourceItem = await HttpHelper.Default.GetMetadataForUrlAsync<DriveItem>(fileUrl, accessToken);;
            string base64 = await GetFileContentAsBase64(fileUrl, accessToken, sourceItem);

            //Glasswall Rebuild
            byte[] regeneratedFileBytes = await RebuildFileWithGlasswall(base64);

            return new FileStreamResult(new MemoryStream(regeneratedFileBytes), sourceItem.File.MimeType)
            {
                FileDownloadName = sourceItem.Name
            };
        }

        private async Task<byte[]> RebuildFileWithGlasswall(string base64)
        {
            var request = new JObject(){
                { "Base64",base64 }
            };
            string payload = JsonConvert.SerializeObject(request);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, SettingsHelper.GwRebuildApi);
            requestMessage.Headers.TryAddWithoutValidation("x-api-key", SettingsHelper.GwRebuildApiKey);
            requestMessage.Headers.TryAddWithoutValidation("Accept", "*/*");
            requestMessage.Headers.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
            requestMessage.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
            requestMessage.Headers.TryAddWithoutValidation("content-type", "application/json");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
            requestMessage.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
            requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var responseMessage = await new HttpClient().SendAsync(requestMessage);
            responseMessage.EnsureSuccessStatusCode();
            var regeneratedFile = await responseMessage.Content.ReadAsStringAsync();
            var regeneratedFileBytes = Convert.FromBase64String(regeneratedFile);
            return regeneratedFileBytes;
        }

        private async Task<string> GetFileContentAsBase64(string fileUrl, string accessToken, DriveItem sourceItem)
        {
            string baseUrl = ActionHelpers.ParseBaseUrl(fileUrl);
            var itemContentUrl = $"https://graph.microsoft.com/v1.0/drives/{sourceItem.ParentReference.DriveId}/items/{sourceItem.Id}/content";
            var contentStream = await HttpHelper.Default.GetStreamContentForUrlAsync(itemContentUrl, accessToken);
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                contentStream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return Convert.ToBase64String(bytes);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var input = GetActivationParameters(filterContext.HttpContext.Request);
            if (input != null)
            {
                if (input.UserId != User.Identity.Name)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }

        #endregion

        public ActionResult GetAsyncJobStatus(string identifier)
        {
            var job = JobTracker.GetJob(identifier);
            return View("AsyncJobStatus", new AsyncActionModel { JobIdentifier = identifier, Status = job });
        }

        
        public static FileHandlerActivationParameters GetActivationParameters(HttpRequestBase request)
        {
            FileHandlerActivationParameters activationParameters = null;
            if (IsFileHandlerActivationRequest(request, out activationParameters))
            {
                return activationParameters;
            }
            return null;
        }

        public static bool IsFileHandlerActivationRequest(HttpRequestBase request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;
            if (request.Form != null && request.Form.AllKeys.Any())
            {
                activationParameters = new FileHandlerActivationParameters(request.Form);
                return true;
            }
            else
            {
                var persistedRequestData = CookieStorage.Load(request);
                if (null != persistedRequestData)
                {
                    activationParameters = new FileHandlerActivationParameters(persistedRequestData);
                    return true;
                }
            }
            return false;
        }

        public static bool IsFileHandlerActivationRequest(Microsoft.Owin.IOwinRequest request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;

            var formTask = request.ReadFormAsync();
            formTask.RunSynchronously();
            var formData = formTask.Result;

            if (formData != null && formData.Any())
            {
                activationParameters = new FileHandlerActivationParameters(null);
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