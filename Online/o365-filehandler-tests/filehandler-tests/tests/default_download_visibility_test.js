/* globals gauge*/
"use strict";


var expect = require('chai').expect;
let page;

// --------------------------
// Gauge step implementations
// --------------------------

beforeStep(async function () {
    // async code for before step
    page = gauge.dataStore.scenarioStore.get('page');
});

step("Validate Default Download Button is Hidden in Action Bar", async () => {

    var defultDownloadButton = "//button[@data-automationid='downloadCommand']";
    const visible = await page.isVisible(defultDownloadButton);
    expect(visible, "Default download button is hidden in Action Bar").to.be.false;

});

step("Validate Default Download Button is Hidden in Context Menu", async () => {

    var defultDownloadButton = "//button[@data-automationid='downloadCommand']";
    const visible = await page.isVisible(defultDownloadButton);
    expect(visible, "Default download button is hidden in Action Bar").to.be.false;

});

step("Validate Default Download Button is Hidden in Preview", async () => {

    var defultDownloadButton = "//button[@data-automationid='download']";
    const visible = await page.isVisible(defultDownloadButton);
    expect(visible, "Default download button is hidden in Action Bar").to.be.false;

});