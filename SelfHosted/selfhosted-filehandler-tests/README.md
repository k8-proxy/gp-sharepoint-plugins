# playwright-automation
Browser automation with https://github.com/microsoft/playwright

# Install Gauge
```
npm install -g @getgauge/cli
```

# Install npm packages
```
npm install
```

# Execute on CI
```
npm install -g @getgauge/cli
cat env/ci/aut.properties
npm install
npm test
```

# Basic Authentication with gauge env/default/auth.properties
```
SP_URL=http://USERNAME:PASSWORD@SELF_HOSTED_SITE_IP
SITE_NAME= The Sharepoint Site Name(Case Sensitive).  E.g. Test Collection Site
```

# Execute Tests
```
npm test
```
https://docs.gauge.org/execution.html
E.g.
```
npm test
gauge run specs
gauge run --env ci specs
gauge run specs/spec_name.spec:scenario_line_number
gauge run specs/file_download_test.spec:11
```

# Test Summary

# File Handler SharePoint Rebuild Test

Description: The purpose of this test is to verify when the user uploads the corrupted file and downloads then it downloads as a clean file to the end-user.

Scope:  Verify when the user downloads a file from the Context menu, Action bar, or page Preview the file download as a clean file.
        To make sure the file is clean compare the downloaded file with the already uploaded clean file.
        This testing performs only for pdf and jpg file types.

1. Upload the Corrupted file then Download it via Context Menu and verify the downloaded file is a clean file.
2. Upload the Corrupted file then Download it via Action Bar and verify the downloaded file is a clean file.
3. Upload the Corrupted file select the uploaded file to preview it and Download from the preview page, then verify the downloaded file is a clean file.

# File Handler OneDrive Rebuild Test

Description: The purpose of this test is to verify when the user uploads the corrupted file to OneDrive and downloads then it downloads as a clean file to the end-user.

Scope:  Verify when the user downloads a file to OneDrive from the Context menu, Action bar, or page Preview the file download as a clean file.
        To make sure the file is clean compare the downloaded file with the already uploaded clean file.
        This testing performs only for pdf and jpg file types.

1. Upload the Corrupted file to OneDrive then Download it via Context Menu and verify the downloaded file is a clean file.
2. Upload the Corrupted file to OneDrive then Download it via Action Bar and verify the downloaded file is a clean file.
3. Upload the Corrupted file to OneDrive select the uploaded file to preview it and Download from the preview page, then verify the downloaded file is a clean file.

# File Handler Site Collection Rebuild Test

Description: The purpose of this test is to verify when the user uploads the corrupted file to selected site and downloads then it downloads as a clean file to the end-user.

Scope:  Verify when the user selects a site and downloads a file from the Context menu, Action bar, or page Preview file download as a clean file.
        To make sure the file is clean compare the downloaded file with the already uploaded clean file.
        This testing performs only for pdf and jpg file types.

1. Upload the Corrupted file to a seleted site then Download it via Context Menu and verify the downloaded file is a clean file.
2. Upload the Corrupted file to a seleted site then Download it via Action Bar and verify the downloaded file is a clean file.
3. Upload the Corrupted file to a selected site then preview the uploaded file and Download it from the preview page, then verify the downloaded file is a clean file.
