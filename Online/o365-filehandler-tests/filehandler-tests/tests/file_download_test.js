/* globals gauge*/
"use strict";

const playwright = require('playwright');
const fs = require("fs");
const path = require("path");
const fc = require('filecompare');
const assert = require('assert');
const headless = process.env.headless.toLowerCase() === 'true';
const browserType = 'chromium'; // ['chromium', 'firefox', 'webkit']
let browser;
let page;
let context;
// --------------------------
// Gauge step implementations
// --------------------------
beforeSpec(async () => {
    //await openBrowser({ headless: headless })
    browser = await playwright[browserType.toString()].launch({ headless: headless, downloadsPath: "./resources/downloaded_files" });
    context = await browser.newContext();
    page = await context.newPage();

    const app_endpoint = process.env.SP_URL
    await page.goto(app_endpoint);
    await page.waitForSelector("//input[@name='loginfmt']");
    await page.type("//input[@name='loginfmt']", process.env.SP_USER); // Types instantly
    await page.click("//input[@value='Next']"); // Click triggers navigation.
    await page.type("//input[@name='passwd']", process.env.SP_USER_PWD); // Types instantly
    await page.click("//input[@value='Sign in']"); // Click triggers navigation.
    await page.click("//input[@value='Yes']"); // Click triggers navigation.

    gauge.dataStore.suiteStore.put('page', page);
    gauge.dataStore.suiteStore.put('context', context);

});

step("Go to the SharePoint home page", async () => {
    const app_endpoint = process.env.SP_URL
    await page.goto(app_endpoint);
});

step("On the choosen SharePoint site", async () => {
    await page.click("//span[text()='SharePoint']");

    const site_name = process.env.SITE_NAME;
    ////const page = gauge.dataStore.suiteStore.get('page');
    // Select sharePoint to navigate to the sharePoint project

    // Select Glass Wall test site
    await page.click("//div[text()='" + site_name + "']")
    await page.waitForTimeout(3000); // wait 3 seconds
});

step("Naviagte to the Documents section", async () => {
    // Navigate to Document tab
    await page.click('text="Documents"');
});

step("Upload file <file_name>", async (file_name) => {
    ////const page = gauge.dataStore.suiteStore.get('page');
    const currentFileName = file_name
    ////console.log("Uploading " + currentFileName);

    // File Upload
    await page.waitForTimeout(3000); // wait 3 seconds
    await page.click("//button[@data-automationid='uploadCommand']");
    await page.waitForTimeout(3000); // wait 3 seconds
    await page.click("//button[@data-automationid='uploadFileCommand']");
    await page.waitForTimeout(3000); // wait 3 seconds

    // Upload Currupted File
    const handle = await page.$('input[type="file"]');
    await handle.setInputFiles('./resources/corrupted_files/' + currentFileName);
    await page.waitForTimeout(3000); // wait 3 seconds
});

step("Select file <file_name>", async (file_name) => {
    const checkBox = "//div[@aria-label='" + file_name + "']/div"
    page.waitForSelector(checkBox)
    // Select file only if not already selected
    const checkBoxElement = await page.$(checkBox);
    var checkBoxClass = await checkBoxElement.getAttribute("class");

    if (!checkBoxClass.includes("is-checked")) {
        //await page.click("//div[@aria-label='" + file_name + "']/div");
        checkBoxElement.click()
    }
    else{
        //console.log("File is already selected")
    }

});

step("Download selected file(s) via Context Menu", async () => {
    ////const page = gauge.dataStore.suiteStore.get('page');
    //const context = gauge.dataStore.suiteStore.get('context');


    //await page.click("//div[@aria-label='" + currentFileName + "']/div");
    await page.click("//div[contains(@class,'is-checked')]/following::i[@data-icon-name='MoreVertical']");
    await page.click("//span[text()= 'Download (Glasswall)']");
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });
    //page.on('download', download => download.path().then(//console.log));

    await page.waitForTimeout(35000); // wait 5 seconds

});

step("Download selected file(s) via Action Bar", async () => {
    ////const page = gauge.dataStore.suiteStore.get('page');
    //const context = gauge.dataStore.suiteStore.get('context');
    //const currentFileName = file_name

    //console.log("Downloading " + currentFileName);

    //await page.click("//div[@aria-label='" + currentFileName + "']/div");
    await page.click("//i[@data-icon-name='More']");
    await page.click("//span[text()= 'Download (Glasswall)']");
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });
    //page.on('download', download => download.path().then(//console.log));

    await page.waitForTimeout(15000); // wait 5 seconds

});


step("Preview file <file_name>", async (file_name) => {
    ////const page = gauge.dataStore.suiteStore.get('page');
    //const context = gauge.dataStore.suiteStore.get('context');
    //const currentFileName = file_name

    //console.log("Downloading " + currentFileName);
    const fileNameSelector = "//button[text()='" + file_name + "']"
    page.waitForSelector(fileNameSelector)

    await page.waitForTimeout(3000); // wait 3 seconds
    await page.click(fileNameSelector);


});


step("Download the Previewed file", async () => {
    const downloadButton= "//span[text()= 'Download (Glasswall)']"
    page.waitForSelector(downloadButton)
    await page.click(downloadButton);
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });
    //page.on('download', download => download.path().then(//console.log));

    await page.waitForTimeout(40000); // wait 20 seconds

});

step("Compare file <file_name> with the clean file", async (file_name) => {
    ////const page = gauge.dataStore.suiteStore.get('page');
    const currentFileName = file_name

    // File Comapare
    //console.log("Comparing " + currentFileName);
    const cb = function (isEqual) {
        //console.log(currentFileName + " File Equal :" + isEqual);
        if (!isEqual) {
            assert.fail(currentFileName + ' is not Equal');
        }
    }
    var path1 = "./resources/downloaded_files/" + currentFileName;
    var path2 = "./resources/cleaned_files/" + currentFileName;


    fc(path1, path2, cb);
    await page.waitForTimeout(3000); // wait 3 seconds


});

step("Delete selected file(s)", async () => {
    //const page = gauge.dataStore.suiteStore.get('page');
    //const currentFileName = file_name

    // let pages = context.pages();
    // page = pages[0]
    // //console.log(await page.title())
    // Navigate to Document tab
    //await page.click('text="Documents"');

    // Select file only if not already selected
    // const checkBoxElement = await page.$("//div[@aria-label='" + currentFileName + "']/div");
    // var checkBoxClass = await checkBoxElement.getAttribute("class");

    // if (!checkBoxClass.includes("is-checked")) {
    //     //await page.click("//div[@aria-label='" + currentFileName + "']/div");
    //     checkBoxElement.click()
    // }

    // Delete selected files
    await page.click("//button[@data-automationid='deleteCommand']");
    await page.click("//button[@data-automationid='confirmbutton']");


    await page.waitForTimeout(5000); // wait 5 seconds

    const directory = './resources/downloaded_files';

    fs.readdir(directory, (err, files) => {
        if (err) throw err;

        for (const file of files) {
            fs.unlink(path.join(directory, file), err => {
                if (err) throw err;
            });
        }
    });

});

step("Close preview", async () => {
    //const page = gauge.dataStore.suiteStore.get('page');
    await page.goBack()
    await page.waitForTimeout(5000); // wait 5 seconds

});
