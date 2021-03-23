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

step("Go to the SharePoint home page", async () => {
    const app_endpoint = process.env.SP_URL
    await page.goto(app_endpoint);
});

step("Naviagte to the Documents section", async () => {
    // Navigate to Document tab
    await page.click('text="Documents"'); 
    await page.waitForTimeout(5000);

});

step("Upload file <file_name>", async (file_name) => {

    const currentFileName = file_name

    // File Upload
    await page.click("//span[text()='Upload']");
    await page.waitForTimeout(3000);
 
    // Upload Currupted File
    await page.waitForTimeout(3000);
    const handle = await page.$('input[type="file"]');
    await page.waitForTimeout(3000);
    await handle.setInputFiles('./resources/corrupted_files/' + currentFileName);
    await page.waitForTimeout(10000);
});

step("Select file <file_name>", async (file_name) => {
    await page.waitForTimeout(5000);

    const checkBox = "(//a[@title='" + file_name + "']/preceding::div[@role='checkbox'])[last()]"
    page.waitForSelector(checkBox)
    // Select file only if not already selected
    const checkBoxElement = await page.$(checkBox);
    var checkBoxClass = await checkBoxElement.getAttribute("class");

    if (!checkBoxClass.includes("is-checked")) {
        checkBoxElement.click()
    }
    else {
        //console.log("File is already selected")
    }

});

step("Download selected file(s) via Context Menu", async () => {

   // await page.click("//div[contains(@class,'is-checked')]/following::i[@data-icon-name='MoreVertical']");
    await page.click("//div[@aria-label='Download']");
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(35000);
});

step("Download selected file(s) via Action Bar", async () => {

    await page.click("//button[@aria-label='Show actions']");
    await page.click("//div[@aria-label='Download']");
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(15000);
});


step("Download the Previewed file <file_name>", async (file_name) => {

    var Extensions = /(\.jpg|\.jpeg|\.png)$/i; 

if (!Extensions.exec(file_name)) { 

    const fileNameSelector = "//div[@data-automationid='DetailsRowFields']//a[@title='" + file_name + "']"
    page.waitForSelector(fileNameSelector)
    await page.click(fileNameSelector);
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });
    await page.waitForTimeout(25000);

    }
    else {

    const fileNameSelector = "//div[@data-automationid='DetailsRowFields']//a[@title='" + file_name + "']"
    page.waitForSelector(fileNameSelector)
    await page.click(fileNameSelector);
    await page.waitForTimeout(5000);
    const downloadButton = "//span[text()='Download']"
    page.waitForSelector(downloadButton)
    await page.click(downloadButton);
    const client = await context.newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });
    await page.waitForTimeout(5000);
    await page.click("//i[@data-icon-name='Cancel']");
    await page.waitForTimeout(20000);
     }

});


step("Compare file <file_name> with the clean file", async (file_name) => {

    const currentFileName = file_name

    const cb = function (isEqual) {
        if (!isEqual) {
            assert.fail(currentFileName + ' is not Equal');
        }
    }
    var path1 = "./resources/downloaded_files/" + currentFileName;
    var path2 = "./resources/cleaned_files/" + currentFileName;


    fc(path1, path2, cb);
    await page.waitForTimeout(3000);

});

step("Delete selected file(s)", async () => {

    await page.waitForTimeout(3000);
    await page.click("//div[@aria-label='Delete']");
    // await page.click("//span[text()= 'Delete']");
    await page.click("//button[@class='od-Dialog-action od-Button']//span[text()='Delete']");

    await page.waitForTimeout(5000);
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


