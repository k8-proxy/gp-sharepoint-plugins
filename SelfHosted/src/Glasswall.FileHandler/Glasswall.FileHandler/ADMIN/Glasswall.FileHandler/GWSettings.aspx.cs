using Microsoft.SharePoint;
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
        string JSONPATH = SPContext.Current.Site.Url + "/_layouts/15/Glasswall.FileHandler/gwkey.txt";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    GWUtility.WriteLog("Handler JSON Url: " + JSONPATH);
                    JObject jsonConfig = JObject.Parse(ReadSettings(JSONPATH));
                    foreach (JProperty prop in jsonConfig.Properties())
                    {
                        GWUtility.WriteLog($"{prop.Name}- {prop.Value}");
                        if (prop.Name == GWConstants.RebuildApiKeySettingsName)
                        {
                            txt_APIKey.Text = prop.Value.ToString();
                            GWUtility.WriteLog($"GW rebuild api key found: {txt_APIKey.Text.Substring(0,3)}xxxxxxxxx.");
                        }
                        if (prop.Name == GWConstants.RebuildApiUrlSettingsName)
                        {
                            txt_APIUrl.Text = prop.Value.ToString();
                            GWUtility.WriteLog($"GW rebuild api url found: {txt_APIUrl.Text}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_status.Text = ex.Message;
                lbl_status.Visible = true;
            }
        }

        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                GWUtility.WriteLog("Saving changes ...");
                if (string.IsNullOrEmpty(txt_APIUrl.Text.Trim()))
                {
                    lbl_status.Text = "Glasswall rebuild api URL cannot be null. Please enter the URL.";
                    lbl_status.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(txt_APIKey.Text.Trim()))
                {
                    lbl_status.Text = "Glasswall rebuild api key cannot be null. Please enter the key.";
                    lbl_status.Visible = true;
                    return;
                }
                JObject gwSettingsJson = new JObject(
                                new JProperty(GWConstants.RebuildApiKeySettingsName, txt_APIKey.Text),
                                new JProperty(GWConstants.RebuildApiUrlSettingsName, txt_APIUrl.Text)
                            );

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SaveSettings(JSONPATH, gwSettingsJson.ToString());
                }
                );

                lbl_status.ForeColor = Color.DarkGreen;
                lbl_status.Text = "Application settings successfully saved."; 
                lbl_status.Visible = true;
            }
            catch (Exception ex)
            {
                lbl_status.ForeColor = Color.Red;
                lbl_status.Text = ex.Message;
                lbl_status.Visible = true;
            }
        }

        private string ReadSettings(string path)
        {
            WebRequest request = WebRequest.Create(path);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
        private void SaveSettings(string path, string data)
        {           
            string sPath = $"{SPUtility.GetCurrentGenericSetupPath(@"TEMPLATE\LAYOUTS")}{GWConstants.SettingsPath}";
            GWUtility.WriteLog($"Saving settings to path:{sPath}");
            File.WriteAllText(sPath, data, Encoding.UTF8);
        }
    }
}
