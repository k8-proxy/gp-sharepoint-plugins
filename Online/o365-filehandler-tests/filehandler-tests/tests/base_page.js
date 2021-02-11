/* globals gauge*/
"use strict";
//const { openBrowser, write, closeBrowser, goto, press, screenshot, text, focus, textBox, toRightOf } = require('taiko');
const assert = require("assert");
const path = require("path");
const headless = process.env.headless.toLowerCase() === 'true';

const playwright = require('playwright');
const browserType = 'chromium'; // ['chromium', 'firefox', 'webkit']
let browser;

// ---------------
// Execution Hooks
// ---------------

//TODO: beforeSpec
// rename scnearios
    //playwright-chomium
//* Preview <file_name>

// Change project_name to site mane
// beforeSuite(async () => {
//     //await openBrowser({ headless: headless })
//     browser = await playwright[browserType.toString()].launch({ headless: headless, downloadsPath: "./resources/downloaded_files" });
//     const context = await browser.newContext();
//     const page = await context.newPage();

//     const app_endpoint = process.env.SP_URL
//     await page.goto(app_endpoint);
//     await page.waitForSelector("//input[@name='loginfmt']");
//     await page.type("//input[@name='loginfmt']", process.env.SP_USER); // Types instantly
//     await page.click("//input[@value='Next']"); // Click triggers navigation.
//     await page.type("//input[@name='passwd']", process.env.SP_USER_PWD); // Types instantly
//     await page.click("//input[@value='Sign in']"); // Click triggers navigation.
//     await page.click("//input[@value='Yes']"); // Click triggers navigation.

//     gauge.dataStore.suiteStore.put('page', page);
//     gauge.dataStore.suiteStore.put('context', context);

// });

afterSuite(async () => {
    //await browser.close();
});

gauge.customScreenshotWriter = async function () {
    // const screenshotFilePath = path.join(process.env['gauge_screenshots_dir'], `screenshot-${process.hrtime.bigint()}.png`);
    // await screenshot({ path: screenshotFilePath });
    // return path.basename(screenshotFilePath);
};

