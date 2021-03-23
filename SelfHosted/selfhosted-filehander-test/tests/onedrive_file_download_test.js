/* globals gauge*/
"use strict";

const fs = require("fs");
const path = require("path");
const fc = require('filecompare');
const assert = require('assert');
let page;
let context;

// --------------------------
// Gauge step implementations
// --------------------------
beforeStep(async function () {
    // async code for before step
    context = gauge.dataStore.scenarioStore.get('context');
    page = gauge.dataStore.scenarioStore.get('page');

});

step("Open hamburg menu", async () => {

    await page.click("(//button[@aria-label='App Launcher'])[1]"); 
    await page.waitForTimeout(5000);
});

step("Select OneDrive", async () => {

   // await page.click("//span[text()='OneDrive']"); 
   await page.click("//a[@aria-label='OneDrive']"); 
    await page.waitForTimeout(5000);
});

step("Download OneDrive file <file_name> via Action Bar", async (file_name) => {

    await page.click("//a[text()='" + file_name + "']/following::button[@aria-label='Show actions']");
    await page.click("//div[@aria-label='Download']");
    const client = await page.context().newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(15000);
});


