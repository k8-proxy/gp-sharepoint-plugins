/* globals gauge*/
"use strict";
const headless = process.env.headless.toLowerCase() === 'true';

const playwright = require('playwright');
const browserType = 'chromium'; // ['chromium', 'firefox', 'webkit']
let browser;
let context;
let page;
const fs = require('fs');

// ---------------
// Execution Hooks
// ---------------

beforeSuite(async () => {
    browser = await playwright[browserType.toString()].launch({ headless: headless, downloadsPath: "./resources/downloaded_files" });
});

afterSuite(async () => {
    // Closes the browser and all of its pages (if any were opened).
    browser.close();
});

beforeSpec(async () => {
    // Store storage state as an env variable
    let authFile = fs.readFileSync('env/ci/auth.json');
    process.env.STORAGE = authFile

    // Create a new context with the saved storage state
    const storageState = JSON.parse(process.env.STORAGE);
    const context = await browser.newContext({ storageState });

    page = await context.newPage();

    // Login to the AUT
    const app_endpoint = process.env.SP_URL
    await page.goto(app_endpoint);
    // await page.waitForSelector("//input[@name='loginfmt']");
    // await page.type("//input[@name='loginfmt']", process.env.SP_USER);
    // await page.click("//input[@value='Next']");
    // await page.type("//input[@name='passwd']", process.env.SP_USER_PWD);
    // await page.click("//input[@value='Sign in']");
    // await page.click("//input[@value='Yes']");
});

afterSpec(async () => {
    // Closes the browser context. All the pages that belong to the browser context will be closed.
    await context.close();
});

beforeScenario(async () => {
    gauge.dataStore.scenarioStore.put('context', context);
    gauge.dataStore.scenarioStore.put('page', page);

});

afterScenario(async () => {

});

beforeStep(async function () {
    // async code for before step
});

afterStep(async function () {
    // After every step, update scenarioStore.page with the current page
    gauge.dataStore.scenarioStore.put('context', context);
    gauge.dataStore.scenarioStore.put('page', page);
});

gauge.customScreenshotWriter = async function () {
    // const screenshotFilePath = path.join(process.env['gauge_screenshots_dir'], `screenshot-${process.hrtime.bigint()}.png`);
    // await screenshot({ path: screenshotFilePath });
    // return path.basename(screenshotFilePath);
};

