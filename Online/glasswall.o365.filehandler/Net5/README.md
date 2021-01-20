# Glasswall File Handler for Download in Office 365

This project provides the implementation of a file handler 2.0 for Microsoft Office 365.
This file handler exposes a custom action, with the **Download (Glasswall)** action added to all files. The action when invoked will read the file content from O365, perform file rebuild using Glasswall rebuild API and then download the file.

## Getting Started

To get started, you need to complete the following actions:

1. Register a new application with Azure Active Directory, generate an app password, and provide a redirect URI for the application.
2. Register the file handler extensions as an add-in for your application in Azure Active Directory.
4. Navigate to OneDrive for Business or a SharePoint document library and allow file handlers to load.
5. Select any file, Click **Download (Glasswall)** action button.

### Register a new application

To register a new application with Azure Active Directory, log into the [Azure Portal](https://portal.azure.com).

After logging into the Azure Portal, the following steps will allow you to register your file handler application:

1. Navigate to the **Azure Active Directory** module.
2. Select **App registrations** and click **New application registration**.
   1. Type the name of your file handler application.
   2. Ensure **Application Type** is set to **Web app / API**
   3. Enter a sign-on URL for your application, for this sample use `https://localhost:44382`.
   4. Click **Create** to create the app.
3. After the app has been created successfully, select the app from the list of applications. It should be at the bottom of the list.
4. Copy the **Application ID** for the app you registered and paste it into appsettings.json file under AzureAd:ClientId setting.
5. Make a note of the **Object ID** for this application, since you will need this later to register the file handler manifest.
6. Configure the application settings for this sample:
   1. Select **Reply URLs** and ensure that `https://localhost:44382/signin-oidc` is listed.
   2. Select **Required Permissions** and then **Add**.
   3. Select **Select an API** and then choose **Microsoft Graph** and click **Select**.
   4. Find the permission **Have full access to all files user can access** and check the box next to it, then click **Select**, and then **Done**.
   5. Select **Keys** and generate a new application key by entering a description for the key, selecting a duration, and then click **Save**. Copy the value of the displayed key since it will only be displayed once.
      * Paste the value of the key you generated into the appsettings.json file in this project, under AzureAd:ClientSecret setting.

### Register the file handler manifest

After registering your app with Azure Active Directory, you can upload the file handler manifest information into the application.

For detailed instructions on how to upload the file handler manifest, see [Registering file handlers](https://docs.microsoft.com/en-us/onedrive/developer/file-handlers/register-manually).

The file handler manifest  is available in the `addin-schema.json` file in this project.

### Update appsettings of FileHandler
Following appsettings values needs to be provided before you can use the file handler:
1. AzureAd:Domain - Domain of your O365 subscription. For e.g. xamariners.com.
2. AzureAd:TenantId - Tenant id of your Azure Active Directory tenant.  
3. Glasswall:BaseUrl - Base url of your Glasswall rebuild api endpoint.
4. Glasswall:ApiKey - Api Key to access the Glasswall rebuild api.

### Run the project and sign-in

Once your project is registered and configured, you're ready to run it. Press F5 to launch the project in the debugger.
The file handler project will load in your default browser and be ready for you to sign in.

### Navigate to OneDrive and use the Markdown file handler

Once you have authorized the file handler to have access, the file handler will be available in OneDrive and SharePoint.
After signing in to the app, click the "Try it in OneDrive" button to launch your OneDrive.
Due to service caches, it may take a few minutes before your file handler shows up in OneDrive.
You may need to close your browser and open it again before the file handler will be activated.

## Next Steps

Now that the file handler is wired up, try downloading a file from SharePoint/OneDrive.

### Download (Glasswall)

The last part of this sample is to use the custom action provided by the file handler. Switch back to OneDrive.
Select a file (click the check) and then in the toolbar, click `Download (Glasswall)`.
This will launch the file handler's custom action handler and download the file.
The file handler code will download the file, rebuild it using Glasswall rebuild api, and download the file to your browser.

## Related references

For more information about file handlers, check out the [OneDrive developer portal](https://dev.onedrive.com).

For more information about Microsoft Graph API, see [Microsoft Graph](https://graph.microsoft.com).
