using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace Glasswall.FileHandler.Layouts.Glasswall.FileHandler
{
    public partial class GWSettings : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    PopulateGlasswallRebuildApiSettings();
                }
                catch (Exception ex)
                {
                    lbl_status.Text = ex.Message;
                    lbl_status.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                GWUtility.WriteLog("Saving Glasswall Rebuild Api settings changes ...");
                if (string.IsNullOrEmpty(txt_APIUrl.Text.Trim()))
                {
                    lbl_status.Text = "Glasswall Rebuild Api Url cannot be null. Please enter the URL.";
                    lbl_status.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(txt_APIKey.Text.Trim()))
                {
                    lbl_status.Text = "Glasswall Rebuild Api Key cannot be null. Please enter the key.";
                    lbl_status.Visible = true;
                    return;
                }

                SaveGlasswallRebuildApiSettings(txt_APIUrl.Text.Trim(), txt_APIKey.Text.Trim());

                lbl_status.ForeColor = Color.DarkGreen;
                lbl_status.Text = "Glasswall Rebuild Api settings saved successfully."; 
                lbl_status.Visible = true;
            }
            catch (Exception ex)
            {
                lbl_status.ForeColor = Color.Red;
                lbl_status.Text = ex.Message;
                lbl_status.Visible = true;
            }
        }

        private void PopulateGlasswallRebuildApiSettings() 
        {
            SPFarm farmObject = SPFarm.Local;
            if (farmObject.Properties != null && farmObject.Properties.Count > 0)
            {
                if (farmObject.Properties.ContainsKey(GWConstants.PROPS_REBUILD_API_URL))
                {
                    txt_APIUrl.Text = Convert.ToString(farmObject.Properties[GWConstants.PROPS_REBUILD_API_URL]);
                    GWUtility.WriteLog($"Glasswall Rebuild Api Url found: {txt_APIUrl.Text}.");
                }
                if (farmObject.Properties.ContainsKey(GWConstants.PROPS_REBUILD_API_KEY))
                {
                    txt_APIKey.Text = Convert.ToString(farmObject.Properties[GWConstants.PROPS_REBUILD_API_KEY]);
                    string strTrimmedKey = txt_APIKey.Text.Trim().Length > 3 ? txt_APIKey.Text.Trim().Substring(0, 3) : txt_APIKey.Text.Trim();
                    GWUtility.WriteLog($"Glasswall Rebuild Api Key found: {strTrimmedKey}xxxxxxxxx.");
                }
            }
        }

        private void SaveGlasswallRebuildApiSettings(string apiUrl, string apiKey)
        {
            SPFarm farmObject = SPFarm.Local;
            if (farmObject.Properties != null)
            {
                farmObject.Properties[GWConstants.PROPS_REBUILD_API_URL] = apiUrl;
                farmObject.Properties[GWConstants.PROPS_REBUILD_API_KEY] = apiKey;
                farmObject.Update();
            }
        }
    }
}
