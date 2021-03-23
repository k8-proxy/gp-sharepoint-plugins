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

