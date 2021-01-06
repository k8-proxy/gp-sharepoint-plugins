using Microsoft.SharePoint;
using System;

namespace Glasswall.FileHandler.GWDocLibEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class GWDocLibEventReceiver : SPItemEventReceiver
    {      
        /// <summary>
        /// When document is uploaded to the library try process the file
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            GWUtility.LoadConfiguration(properties.Site.Url);
        }

        /// <summary>
        /// When document finished upload try process the file, this happens when uploaded from Windows Explorer
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);           
            GWUtility.WriteLog($"GW File Updated Event URL:{properties.ListItem.File.Url} Length:{properties.ListItem.File.Length}");
            try
            {                
                properties.Web.AllowUnsafeUpdates = true;
                EventFiringEnabled = false;
                var fileItem = properties.ListItem.File;
                if (fileItem != null)
                {
                    GWUtility.WriteLog("GW File URL processing ..." + fileItem.Url);
                    System.Threading.Thread.Sleep(2000);
                    byte[] fileContent = fileItem.OpenBinary();
                    System.Threading.Thread.Sleep(2000); // not mandatory but if mp4 file of large size is uploaded it takes seconds to read 
                    string base64 = Convert.ToBase64String(fileContent);
                    byte[] regeneratedFileBytes = new GlasswallRebuildClient().RebuildFile(base64);
                    SPItem item = properties.ListItem;
                    properties.ListItem["Title"] = "GW_" + fileItem.Name; // just updating the Title to make sure it is processed
                    properties.ListItem.SystemUpdate();
                    fileItem.SaveBinary(regeneratedFileBytes);
                    GWUtility.WriteLog("GW file done :" + properties.ListItem.File.Length);
                }
            }
            catch (Exception ex)
            {
                GWUtility.WriteLog("GW Upload Exception: " + ex.Message + ex.StackTrace);

            }
            finally
            {
                properties.Web.AllowUnsafeUpdates = false;
                EventFiringEnabled = true;
            }
        }
        

    }
}