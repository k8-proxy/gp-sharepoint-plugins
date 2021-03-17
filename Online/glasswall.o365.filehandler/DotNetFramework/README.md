# Introduction 
Glasswall File Handler - .NET Framework Version

This project provides a sample implementation of a file handler 2.0 for Microsoft Office 365 (Sharepoint Online, OneDrive Online).
This sample illustrates a custom action, with the **Glasswall Download** action added to all files. The file handler allows downloading a file by rebuilding the file using Glasswall Rebuild Api.

 **Note that this handler has been superseded by the Net5 version** 


## Getting Started

To get started with the sample, you need to complete the following actions:

1. Register a new application with Azure Active Directory, generate an app password, and provide a redirect URI for the application.
2. Register the file handler extensions as an add-in for your application in Azure Active Directory.
3. Run the sample project and sign-in with your Office 365 account and accept the consent prompt so the file handler is registered for your account.
4. Navigate to OneDrive for Business or a SharePoint document library and allow file handlers to load.
5. Select any file. Then Click on the custom action button to download the selected file by rebuilding it using Glasswall rebuild api.

### Register a new application

To register a new application with Azure Active Directory, log into the [Azure Portal](https://portal.azure.com).

After logging into the Azure Portal, the following steps will allow you to register your file handler application:

1. Navigate to the **Azure Active Directory** module.
2. Select **App registrations** and click **New application registration**.
   1. Type the name of your file handler application.
   2. Ensure **Application Type** is set to **Web app / API**
   3. Enter a sign-on URL for your application, for this sample use `https://localhost:44362`.
   4. Click **Create** to create the app.
3. After the app has been created successfully, select the app from the list of applications. It should be at the bottom of the list.
4. Copy the **Application ID** for the app you registered and paste it into the Web.config file on the line: `<add key="ida:ClientId" value="application id here" />`
5. Make a note of the **Object ID** for this application, since you will need this later to register the file handler manifest.
6. Configure the application settings for this sample:
   1. Select **Reply URLs** and ensure that `https://localhost:44362` is listed.
   2. Select **Required Permissions** and then **Add**.
   3. Select **Select an API** and then choose **Microsoft Graph** and click **Select**.
   4. Find the permission **Have full access to all files user can access** and check the box next to it, then click **Select**, and then **Done**.
   5. Select **Keys** and generate a new application key by entering a description for the key, selecting a duration, and then click **Save**. Copy the value of the displayed key since it will only be displayed once.
      * Paste the value of the key you generated into the Web.config file in this project, inside the value for the line: `<add key="ida:ClientSecret" value="put application key here" />`

### Register the file handler manifest

After registering your app with Azure Active Directory, you can upload the file handler manifest information into the application.

For detailed instructions on how to upload the file handler manifest, see [Registering file handlers](https://docs.microsoft.com/en-us/onedrive/developer/file-handlers/register-manually).

The file handler manifest for this sample  is available in the `addin-schema.json` file in this project.

### Register an Azure Storage instance

The sample uses an Azure Storage table to cache user and token information.
To successfully run the sample, you must provide an Azure Storage connection string in Web.config:

```xml
<add key="StorageConnectionString" value="[AzureStorageConnectionString]" />
```

To create a new storage instance and copy the connection string:

1. Open the [Azure Portal](https://portal.azure.com) in your browser. You must have an Azure Subscription or trial subscription.
2. Select **Storage Accounts** and then click **Add**.
3. Name the storage account, leaving the defaults as they are. You will also need to create a new resource group and provide a name.
4. Click **Create** to provision the storage account.
5. After the storage account has finished provisioning, select it from the list of storage accounts. Click on the **Access Keys** tab, and copy the connection string for **key1**.
6. Paste this connection string where `[AzureStorageConnectionString]` appears in Web.config.

### Glasswall Rebuild Api Key
This sample connects to one of the Glasswall Rebuild Api. You will need an Api Key to rebuild the files. Reach out to Glasswall Engineering to know how to get the Api Key. To successfully run the sample, you must provide an Api Key in Web.config:
```xml
<add key="GwRebuildApiKey" value="[ApiKey]"/>
```

### Run the project and sign-in

Once your project is registered and configured, you're ready to run it. Press F5 to launch the project in the debugger.
The file handler project will load in your default browser and be ready for you to sign in.
Sign in to the file handler project, and authorize the application to have access to the data in your OneDrive.

### Navigate to OneDrive and use the Markdown file handler

Once you have authorized the file handler to have access, the file handler will be available in OneDrive and SharePoint.
After signing in to the app, click the "Try it in OneDrive" button to launch your OneDrive.
Due to service caches, it may take a few minutes before your file handler shows up in OneDrive.
You may need to close your browser and open it again before the file handler will be activated.

## Next Steps

### Download file with Glasswall Rebuild

To use the custom action provided by the file handler, perform the following actions:
- Navigate to OneDrive/Sharepoint online.
- Select a file (click the check) and then in the toolbar, click `...` and then **Glasswall Download**.
- This will launch the file handler's custom action handler
- The file handler code will:
    - Read the selected file's content from OneDrive/Sharepoint
    - Rebuild it with Glasswall Rebuild Api 
    - Download the file onto your machine.
