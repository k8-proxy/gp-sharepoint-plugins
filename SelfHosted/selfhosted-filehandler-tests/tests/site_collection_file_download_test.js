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
   // context = gauge.dataStore.scenarioStore.get('context');
    page = gauge.dataStore.scenarioStore.get('page');

});

step("Naviagte to SharePoint Home page", async () => {

    await page.click("(//span[text()='SharePoint'])[2]");

});


step("Naviagte to Choosen site Documents section", async () => {

    const site_name = process.env.SITE_NAME;
    const [newPage] = await Promise.all([
         page.context().waitForEvent('page'),
         page.click("//div[text()='" + site_name + "']") // Opens a new tab
         ])
         await newPage.waitForLoadState();
         console.log(await newPage.title());
         // page = newPage;
         gauge.dataStore.scenarioStore.put('page', newPage);
         

    await newPage.click("//div[text()='Documents']"); 
    await newPage.waitForLoadState();
});