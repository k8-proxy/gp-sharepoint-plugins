using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Glasswall.FileHandler.Features.GWFileHandlerFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("395f1c2e-4d96-477b-bb9f-5f4194132aff")]
    public class GWFileHandlerFeatureEventReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Add th web.config entry for the file download handler
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            if (webApp == null) return;
            AddGlasswallModification(webApp);
            AddExtensionsModification(properties, webApp);
            webApp.Update();
            webApp.WebService.ApplyWebConfigModifications();
        }

       

        /// <summary>
        /// Remove the web.config entries once deactivated
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            string asmDetails = typeof(DownloadFileHandler).AssemblyQualifiedName;

            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            if (webApp == null) return;

            List<SPWebConfigModification> configModsFound = new List<SPWebConfigModification>();
            Collection<SPWebConfigModification> modsCollection = webApp.WebConfigModifications;
            for (int i = 0; i < modsCollection.Count; i++)
            {
                if (modsCollection[i].Owner == asmDetails)
                {
                    configModsFound.Add(modsCollection[i]);
                }
            }

            if (configModsFound.Count > 0)
            {
                foreach (SPWebConfigModification mod in configModsFound)
                    modsCollection.Remove(mod);

                webApp.Update();
                webApp.WebService.ApplyWebConfigModifications();
            }
        }

        private static void AddExtensionsModification(SPFeatureReceiverProperties properties, SPWebApplication webApp)
        {
            string extensions = properties.Feature.Properties["HandleExtensions"].Value;
            if (!string.IsNullOrEmpty(extensions))
            {
                var extensionList = extensions.Split(';');
                foreach (string extn in extensionList)
                {
                    string[] extnTokens = extn.Split('|');
                    if (extnTokens.Length > 1)
                    {
                        string extnName = extnTokens[0];
                        string extnFile = extnTokens[1];

                        var extensionModification = GetConfigModification(extnName, extnFile);
                        webApp.WebConfigModifications.Add(extensionModification);
                    }
                }
            }
        }

        private void AddGlasswallModification(SPWebApplication webApp)
        {
            SPWebConfigModification modification = GetConfigModification("GlasswallDownloadHandler", "download.aspx");
            webApp.WebConfigModifications.Add(modification);
        }

        private static SPWebConfigModification GetConfigModification(string name, string path)
        {
            string fileHandlerTypeName = typeof(DownloadFileHandler).AssemblyQualifiedName;
            SPWebConfigModification modification = new SPWebConfigModification($"add[@name=\"{name}\"]", "configuration/system.webServer/handlers");
            modification.Value = $"<add name=\"{name}\" verb=\"*\" path=\"{path}\" type=\"{fileHandlerTypeName}\" preCondition=\"integratedMode\" />";

            modification.Sequence = 1;
            modification.Owner = fileHandlerTypeName;
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            return modification;
        }
    }
}
