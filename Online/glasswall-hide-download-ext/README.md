**Glasswall Hide Download - SharePoint Framework(SPFx) Extension**

# Overview

This solution contains SPFx Extension of Application Customizer type. It executes script to hide the default Download button for SharePoint online (Office365) Site.

# Getting Started

## Clone the repository
- Open the command-prompt.
- Navigate to local development folder.
- Clone the repository using command, git clone 'https://xamariners@dev.azure.com/xamariners/Glasswall/_git/spcloud-modern-customisation'.

## Install dependencies
- Open 'spcloud-modern-customisation\glasswall-hide-download-ext\config\serve.json' file in VS Code or any other editor.
- Change the PageUrl attribute with your Office365 tenant SharePoint site page where you would like to test.
- Now open the command-prompt.
- Navigate to repo folder 'spcloud-modern-customisation'.
- Navigate to solution folder 'glasswall-hide-download-ext'.
- Run 'npm install'.

## Test your SPFx Extension
- Run 'gulp trust-dev-cert'.
- Run 'gulp serve'.
- This will open your tenant site and may ask you to login.
- Click on 'Load debug Script' to run your extension.

![Load debug script](https://docs.microsoft.com/en-us/sharepoint/dev/images/ext-com-accept-debug-scripts.png)

## Create App Catalog Site
- Go to the SharePoint Admin Center by entering the following URL in your browser. https://{tenant}-admin.sharepoint.com.
- In the left sidebar, select More features.
- Locate the section Apps and select Open.
- On the Apps page, select App Catalog.
- Select OK to create a new app catalog site.
- On the next page, enter the following details:
    1. Title: Enter app catalog.
    2. Web Site Address suffix: Enter your preferred suffix for the app catalog; for example: apps.
    3. Administrator: Enter your username, and then select the resolve button to resolve the username.
    4. Select OK to create the app catalog site.
- SharePoint creates the app catalog site, and you can see its progress in the SharePoint admin center.
- Click [here](https://docs.microsoft.com/en-us/sharepoint/dev/spfx/set-up-your-developer-tenant#create-app-catalog-site) for more info.

## Build and Deploy Package Manually

### Build Solution Package
- Open the command-prompt.
- Navigate to solution folder 'glasswall-hide-download-ext'.
- Run 'gulp clean'.
- Run 'gulp bundle --ship'.
- Run 'gulp package-solution --ship'.
- This will create SharePoint Package File at location '{your-local-development-folder}.\spcloud-modern-customisation\glasswall-hide-download-ext\sharepoint\solution\glasswall-hide-download-ext.sppkg'.

### Deploy Package Mannually to App Catalog Site
- Open your tenant's app catalog site (i.e https://{tenant}.sharepoint.com/sites/apps).
- Click on 'Apps for SharePoint' on Left Navigation Menu.
- Click on Upload and Browse the SPFx package created at earlier step from your local drive.
- Click on 'Deploy' to deploy the solution tenant-wide.

### Verify Solution
- Navigate to SharePoint Online (Office365) Site.
- Open any Document Library.
- Select any file (click on the checkbox) and then in the toolbar, you will not see the default 'Download' button anymore.

## Prepare Office365 Tenant for PNP Microsoft365 Management Shell (Microsoft365 CLI)
- Open the command-prompt.
- Install Microsoft365 CLI using command, 'npm install -g @pnp/cli-microsoft365'.
- Run the following command: m365 login.
- A login device code will be displayed along with a link to a web page where it needs to be entered.
- Navigate to [https://microsoft.com/devicelogin](https://microsoft.com/devicelogin).
- Enter the code into the input field and select Next. 
- You will then be presented with either a login screen or accounts that you have already logged in to Microsoft 365 with. Login with a global admin account.
- You will now be prompted to consent PNP Microsoft365 Management Shell.
- Tick the checkbox 'Consent on behalf of your organisation' if it is visible
- Select Accept to consent and complete the sign-in process.
- Click [here](https://pnp.github.io/cli-microsoft365/user-guide/connecting-office-365/) for more information.

## Build and Deploy Package through GitHub Actions

### Configure Secrets
- On GitHub, navigate to the main page of the repository.
- Under your repository name, click Settings.
- In the left sidebar, click Environments.
- Click on the Environment you're working with.
- Update the following secrets listed under Environment Secrets:
    1. tenant: Tenant name in https://{tenant}.sharepoint.com.
    2. catalogsite: Server relative Path of the App Catalog Site eg sites/apps.
    3. username: Username of the user with administrative permissions on the tenant.
    4. password: Password of the user with administrative permissions on the tenant.

### Build and deploy package
- On GitHub, navigate to the main page of the repository.
- Under your repository name, click Actions.
- In the left sidebar, click on 'SPFx CI-CD with Microsoft365 CLI'
- You will see option as 'Run workflow' on screen.
- Click on 'Run workflow' button to start workflow which build and deploy the package.

## Edit GitHub Action
- On GitHub, navigate to the main page of the repository.
- Under your repository name, click Actions.
- In the left sidebar, click on 'SPFx CI-CD with Microsoft365 CLI'
- Select the three dots(...) besides the latest workflow run item row and then click on 'view workflow file'.
- Click on Edit button to edit the yml workflow file.
- Once updated, click on 'Start Commit' to commit your changes to the repository.

## Build and Deploy Package through CI/CD on DevOps

### Configure CI/CD Release Variables
- Open the Release 'Glasswall-Office365-Hide-Download-CI-Release' on DevOps (tbd).
- Click on Edit.
- Click on Variables Tab.
- Update the following variables as per your environment specs:
    1. tenant: Tenant name in https://{tenant}.sharepoint.com.
    2. catalogsite: Server relative Path of the App Catalog Site eg sites/apps.
    3. username: Username of the user with administrative permissions on the tenant.
    4. password: Password of the user with administrative permissions on the tenant.

### Build Package through CI/CD Pipeline
- Open the Pipeline 'Glasswall-Office365-Hide-Download-CI' on DevOps (tbd).
- Click on Run pipeline.

### Deploy Package through CI/CD Release
- Open the Release 'Glasswall-Office365-Hide-Download-CI-Release' on DevOps (tbd).
- Open latest Release and Re-Deploy OR Create New Release and Deploy.

## Configure Hide Download Extension Solution
SharePoint Online keeps tenant-wide extension details in a List called, "Tenant Wide Extensions" on App Catalog Site.
For Hide-Downlaod-Extension, we keep following two properties which can be configured:
    1. CommandName: This is the comma separated values of all the possible values of attribute 'data-automationid' of HTML controls. We use it to track the download button on the page.
    2. ExecuteFrequency: The is numeric value which defines the frequency interval at which the extension checks for download buttons on page and hides it.

### Steps to update Hide Download Extension Properties
The default values are already configured however if we would like to customize, we can update them as follows:
- Navigate to App Catalog Site on your tenant (i.e. https://{tenant}.sharepoint.com/sites/apps).
- Click on 'Site contents' in the Left Navigation.
- Click on 'Tenant Wide Extensions' List.
- Edit the item with title, 'Hide Default Download'.
- You can make changes in the field called "Component Properties" to update extension properties. 
- Make sure you ONLY update highlighted values as follows and keep the JSON format as it is:
    * {"commandName":"<b><i>download,downloadCommand</i></b>","executeFrequency":<b><i>300</i></b>}
- Please note that the changes to the list will be applied to the extension on your tenant in 2 to 24 hours.
