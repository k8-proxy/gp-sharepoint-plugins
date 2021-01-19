using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.O365.FileHandler.App.Extensions;
using Glasswall.O365.FileHandler.App.Services;
using Glasswall.O365.FileHandler.Models;
using Glasswall.O365.FileHandler.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace Glasswall.O365.FileHandler.Controllers
{
    [Authorize]
    public class FileHandlerController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly IFileRebuilder _fileRebuilder;

        public FileHandlerController(GraphServiceClient graphServiceClient, IFileRebuilder fileRebuilder)
        {
            _graphServiceClient = graphServiceClient;
            _fileRebuilder = fileRebuilder;
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        public async Task<FileStreamResult> Download()
        {
            var parameters = GetActivationParameters(Request);

            var fileUri = new Uri(parameters.ItemUrls.First());
            var itemRequestBuilder = GetItemRequestBuilder(fileUri);
            var sourceItem = await itemRequestBuilder.Request().GetAsync();
            var contentStream = await itemRequestBuilder.Content.Request().GetAsync();
            var base64String = ConvertStreamToBytes(contentStream);

            byte[] regeneratedFileBytes = await _fileRebuilder.Rebuild(base64String);

            return new FileStreamResult(new MemoryStream(regeneratedFileBytes), sourceItem.File.MimeType)
            {
                FileDownloadName = sourceItem.Name
            };
        }

        private static string ConvertStreamToBytes(Stream contentStream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                contentStream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            var base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private IDriveItemRequestBuilder GetItemRequestBuilder(Uri fileUri)
        {
            if (fileUri.Segments[2] == "me/")
            {
                return GetMeDriveItemRequestBuilder(fileUri);
            }
            else if (fileUri.Segments[2] == "drives/")
            {
                return GetDrivesItemRequestBuilder(fileUri);
            }
            else if (fileUri.Segments[2] == "shares/")
            {
                return GetSharesItemRequestBuilder(fileUri);
            }
            IDriveItemRequestBuilder itemRequestBuilder = default;
            return itemRequestBuilder;
        }

        private IDriveItemRequestBuilder GetSharesItemRequestBuilder(Uri fileUri)
        {
            var driveSegment = fileUri.Segments[3];
            var driveId = driveSegment.Substring(0, driveSegment.Length - 1);
            return _graphServiceClient.Shares[driveId].DriveItem;
        }

        private IDriveItemRequestBuilder GetDrivesItemRequestBuilder(Uri fileUri)
        {
            
            var driveSegment = fileUri.Segments[3];
            var driveId = driveSegment.Substring(0, driveSegment.Length - 1);
            var itemId = fileUri.Segments[5];
            return _graphServiceClient.Drives[driveId].Items[itemId];
        }

        private IDriveItemRequestBuilder GetMeDriveItemRequestBuilder(Uri fileUri)
        {
            var itemId = fileUri.Segments[fileUri.Segments.Length - 1];
            return _graphServiceClient.Me.Drive.Items[itemId];
        }

        private FileHandlerActivationParameters GetActivationParameters(HttpRequest request)
        {
            FileHandlerActivationParameters activationParameters = null;
            if (request.IsFileHandlerActivationRequest(out activationParameters))
            {
                return activationParameters;
            }
            return null;
        }

        
    }
}
