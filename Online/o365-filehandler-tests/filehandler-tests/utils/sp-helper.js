async function navigateToSP(page) {
    await page.goto(process.env.SP_URL);
}

async function login(page) {
    await page.waitForSelector("//input[@name='loginfmt']");
    await page.type("//input[@name='loginfmt']", process.env.SP_USER); // Types instantly
    await page.click("//input[@value='Next']"); // Click triggers navigation.
    await page.type("//input[@name='passwd']", process.env.SP_USER_PWD); // Types instantly
    //await page.waitForSelector("//input[@value='Sign in']");
    await page.click("//input[@value='Sign in']"); // Click triggers navigation.
    await page.click("//input[@value='Yes']"); // Click triggers navigation.
    //await page.waitForTimeout(3000); // wait 3 seconds
}

async function uploadFile(page, filePath) {
    await page.click("//span[text() ='Upload']");
    await page.click("//span[text() ='Files']");
    const handle = await page.$('input[type="file"]');
    await handle.setInputFiles(filePath);
}

module.exports = {
    login: login,
    navigateToSP: navigateToSP,
    uploadFile: uploadFile
}