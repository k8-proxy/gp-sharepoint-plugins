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
    page = gauge.dataStore.scenarioStore.get('page');

});

step("Go to the SharePoint home page", async () => {
    const app_endpoint = process.env.SP_URL
    await page.goto(app_endpoint);
});

step("On the choosen SharePoint site", async () => {
    await page.click("//span[text()='SharePoint']");

    // select site 
    const site_name = process.env.SITE_NAME;

    await page.click("//div[text()='" + site_name + "']")
    await page.waitForTimeout(3000); // wait 3 seconds
});

step("Naviagte to the Documents section", async () => {
    // Navigate to Document tab
    await page.click('text="Documents"');
});

step("Upload file <file_name>", async (file_name) => {

    const currentFileName = file_name

    // File Upload
    await page.waitForTimeout(3000);
    await page.click("//button[@data-automationid='uploadCommand']");
    await page.waitForTimeout(3000);
    await page.click("//button[@data-automationid='uploadFileCommand']");
    await page.waitForTimeout(3000);

    // Upload Currupted File
    const handle = await page.$('input[type="file"]');
    await handle.setInputFiles('./resources/corrupted_files/' + currentFileName);
    await page.waitForTimeout(10000);
});

step("Select file <file_name>", async (file_name) => {
    await page.waitForTimeout(5000);

    const checkBox = "//div[@aria-label='" + file_name + "']/div"
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

    await page.click("//div[contains(@class,'is-checked')]/following::i[@data-icon-name='MoreVertical']");
    await page.click("//span[text()= 'Download (Glasswall)']");
    const client = await page.context().newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(35000);
});

step("Download selected file(s) via Action Bar", async () => {

    await page.click("//i[@data-icon-name='More']");
    await page.click("//span[text()= 'Download (Glasswall)']");
    const client = await page.context().newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(15000);
});


step("Preview file <file_name>", async (file_name) => {

    const fileNameSelector = "//button[text()='" + file_name + "']"
    page.waitForSelector(fileNameSelector)

    await page.waitForTimeout(3000);
    await page.click(fileNameSelector);


});


step("Download the Previewed file", async () => {
    const downloadButton = "//span[text()= 'Download (Glasswall)']"
    page.waitForSelector(downloadButton)
    await page.click(downloadButton);
    const client = await page.context().newCDPSession(page);
    await client.send("Page.setDownloadBehavior", { behavior: "allow", downloadPath: "./resources/downloaded_files" });

    await page.waitForTimeout(20000);
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

    await page.click("//button[@data-automationid='deleteCommand']");
    await page.click("//button[@data-automationid='confirmbutton']");


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

step("Close preview", async () => {
    await page.goBack()
    await page.waitForTimeout(5000);

});
