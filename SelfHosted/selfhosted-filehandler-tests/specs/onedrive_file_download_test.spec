OneDrive File Handler Test Specification
==========================================

tags: onedrive-file-handler
|file_name                          |
|-----------------------------------|
|TestAutoCurImage.jpg               |
|TestAutoCurExecuteJavaScriptPDF.pdf|


Download OneDrive file via Context Menu
------------------------------------------------------------
tags: onedrive-file-handler, onedrive-context-menu


* Open hamburg menu
* Select OneDrive
* Upload file <file_name>
* Select file <file_name>
* Download selected file(s) via Context Menu
* Compare file <file_name> with the clean file
* Delete selected file(s)


Download OneDrive file via Action Bar
------------------------------------------------------------
tags: onedrive-file-handler, onedrive-action-bar


* Open hamburg menu
* Select OneDrive
* Upload file <file_name>
* Select file <file_name>
* Download OneDrive file <file_name> via Action Bar
* Compare file <file_name> with the clean file
* Delete selected file(s)


Download the OneDrive Previewed file
------------------------------------------------------------
tags: onedrive-file-handler, onedrive-preview-page


* Open hamburg menu
* Select OneDrive
* Upload file <file_name>
* Download the Previewed file <file_name>
* Compare file <file_name> with the clean file
* Select file <file_name>
* Delete selected file(s)


____________________________________
Teardown steps are here

* Go to the SharePoint home page