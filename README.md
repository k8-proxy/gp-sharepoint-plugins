# Introduction 
This repository contains a collection of Sharepoint Solutions and Extensions that provides Sharepoint integration for the Glasswall Rebuild Solution, both self hosted and online.


## ONLINE

### File Handler
This project provides a sample implementation of a file handler 2.0 for Microsoft Office 365.
This sample illustrates a custom action, with the **Glasswall Download** action added to all files. The file handler allows downloading a file by rebuilding the file using Glasswall Rebuild Api.

The solution is both available for .Net Framework and .Net (Core & 5x)

### Hide Download Extension
This solution contains SPFx Extension of Application Customizer type. It executes script to hide the default Download button for SharePoint online (Office365) Site.

### Download Extension (Work In Progress)
This solution contains SPFx Extension of Application Customizer type. It executes script to insert a custom Download button for SharePoint online (Office365) Site, allowinf full control on its action.


### Embed Script Extension
todo

## SELF-HOSTED
The file handler allows downloading and uploading a file by rebuilding the file using Glasswall Rebuild Api. The rebuild engine endpoint as well as the file types to trigger are configurable.