using System;
using System.Web;
using System.IO;
using Microsoft.SharePoint;
using System.Net.Mime;

namespace Glasswall.FileHandler
{
    public class DownloadFileHandler : IHttpHandler
    {
       public DownloadFileHandler()
        {
            GWUtility.LoadConfiguration(SPContext.Current.Site.Url);
        }       
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// This method will process downloading the documents and sanitise using the REST API
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            string urlPath = context.Request.Url.ToString();

            GWUtility.WriteLog("Processing ... " + urlPath);
            try
            {
                /// This is to handle the download.aspx which is triggered by Download Button
                if (!string.IsNullOrEmpty(context.Request.QueryString["SourceUrl"]))
                {
                    urlPath = BuildDocumentUrl(context);
                }                
                string fileName = Path.GetFileName(urlPath);
                GWUtility.WriteLog(" Filename: " + fileName);
                string base64 = GetFileContentAsBase64(urlPath);
                byte[] regeneratedFileBytes = new GlasswallRebuildClient().RebuildFile(base64);

                context.Response.Clear();
                context.Response.ClearHeaders();
                context.Response.Buffer = true;
                context.Response.ContentType = MediaTypeNames.Application.Octet;
                context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                context.Response.AddHeader("content-length", regeneratedFileBytes.Length.ToString());
                context.Response.BinaryWrite(regeneratedFileBytes);
                context.Response.Flush();
                context.Response.Close();               
            }
            catch (Exception ex)
            {
                GWUtility.WriteLog($@"Exception: {ex.Message}\n{ex.StackTrace}");
                context.Response.ContentType = MediaTypeNames.Text.Html;
                context.Response.Write(PrepareErrorMarkup(urlPath, ex));
            }
        }

        private static string PrepareErrorMarkup(string urlPath, Exception ex)
        {
            var markupBuilder = new StringBuilder();
            markupBuilder.Append("<html>");
            markupBuilder.Append("<head>");
            markupBuilder.Append("<style>body{font-family: Arial, Helvetica, sans-serif;font-size: 13px;}.error{border: 1px solid;margin: 10px 0px;padding: 15px 10px 15px 50px;background-repeat: no-repeat;background-position: 10px center;color: #D8000C;background-color: #FFBABA;}</style>");
            markupBuilder.Append("</head>");
            markupBuilder.Append("<body>");
            markupBuilder.Append("<div class=\"error\">");
            markupBuilder.Append("<h3>Error downloading file</h3>" +
                                 $"<br>File:{Path.GetFileName(urlPath).ToLower()}" +
                                 $"<br>Error:{ex.Message}" +
                                 $"<br><br><a href={SPContext.Current.Site.Url}>Back to site</a>");
            return markupBuilder.ToString();
        }

        private static string BuildDocumentUrl(HttpContext context)
        {
            string urlPath;
            var redirectURL = context.Request.QueryString["SourceUrl"];
            GWUtility.WriteLog("SourceUrl ... " + redirectURL);
            var currentSiteUrl = new Uri(SPContext.Current.Site.Url, UriKind.Absolute);
            var spDomain = string.Format("{0}://{1}:{2}", currentSiteUrl.Scheme, currentSiteUrl.Host, currentSiteUrl.Port);
            urlPath = spDomain + redirectURL;
            GWUtility.WriteLog("DocumentUrl ... " + urlPath);
            return urlPath;
        }

        private string GetFileContentAsBase64(string filePath)
        {
            SPFile file = SPContext.Current.Web.GetFile(filePath);
            byte[] content = file.OpenBinary();
            return Convert.ToBase64String(content);
        }

    }
}
