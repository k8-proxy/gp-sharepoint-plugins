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
                    String redirectURL = context.Request.QueryString["SourceUrl"];
                    string docUrl = SPContext.Current.Site.Url + redirectURL;
                    urlPath = docUrl;
                }                
                string base64 = GetFileContentAsBase64(urlPath);
                string fileName = Path.GetFileName(urlPath);
                GWUtility.WriteLog(" Filename: " + fileName);
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
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                context.Response.Write($@"<b>Error downloading file</b>\n File:{Path.GetFileName(urlPath).ToLower()}\n Error:{ex.Message}");
            }
        }
        private string GetFileContentAsBase64(string filePath)
        {
            SPFile file = SPContext.Current.Web.GetFile(filePath);
            byte[] content = file.OpenBinary();
            return Convert.ToBase64String(content);
        }

    }
}
