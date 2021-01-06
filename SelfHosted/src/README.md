# Introduction 
This project code is based on VS 2019 SharePoint 2019 Empty Project template. Install Visual Studio 2019 with SharePoint development 

# Getting Started
The code contains below features and elements:
1.	GWCustomActionCAFeature - Features activated for CA to show custom settings
2.	GWDocLibFeature - Document Library Event Receiver at web level
3.	GWFileHandlerFeature - Feature at Web Application to change web.config for HTTPHandler
4.	GWCustomActions - Elements for CA custom links and Application Page
5.	GWDocLibEventReceiver - Event Receiver

# Build and Test
This is a Farm Solution project. Before deploy to SharePoint environment change the "Site URL" to your SP site url under Project Properties. Right click and Deploy project. All Features will be activated by default.

# Configurations
1.	To change the API url and API key look for GWKey.txt file under Layouts/Glasswall.FileHandler folder.
2.	To add or remove file extensions look for GWFileHandlerFature.Template.xml file. <Property Key="" Value="" /> contains the extensions for the file to handle for httphandler. Default value is (<Property Key="HandleExtensions" Value="PDFHandler|*.pdf;JPGHandler|*.jpg;DocHandler|*.docx"/>)


