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
# Remove default download Button Tests 

Description: The purpose of this test to check default download button is not visible to the end-user.

Scope: Verify default download button is not visible Context menu, Action bar, and page Preview.
       This test only for pdf and jpg file types.

1. Tested default download button is hidden from the user on the Action Bar.
2. Tested default download button is not visible to the user on the Context Menu.
3. Validate default download button is hidden for user on file preview.

# File Handler Test Specification

Description: The purpose of this test is to verify when the user uploads the corrupted file and downloads then it downloads as a clean file to the end-user.

Scope:  Verify when the user downloads a file from the Context menu, Action bar, or page Preview the file download as a clean file.
        To make sure the file is clean compare the file with the already uploaded clean file.
        This testing performs only for pdf and jpg file types.

1. Upload the Curruted file then Download it via Context Menu and verify the downloaded file is a clean file.
2. Upload the Curruted file then Download it via Action Bar and verify the downloaded file is a clean file.
3. Upload the Corrupted file select the uploaded file to preview it and Download from the preview page, then verify the downloaded file is a clean file.
