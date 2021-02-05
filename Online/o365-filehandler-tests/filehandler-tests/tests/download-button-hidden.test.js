const { chromium } = require('playwright-chromium');
const sphelper = require('../utils/sp-helper');

const fileToUpload = 'resources/cleaned_files/TestAutoCurImage.jpg';

let browser;
let page;

beforeAll(async () => {
    browser = await chromium.launch();
    page = await browser.newPage();
    await sphelper.navigateToSP(page);
    await sphelper.login(page);
    await sphelper.uploadFile(page, fileToUpload)
});

beforeEach(async () => {
    
});

describe('Default Download Button', () => {
    it('Should be hidden in action bar', async () => {
        //select uploaded file
        //check for download button in action bar     
        expect(1 == 1).toBe(true);
    });

    it('Should be hidden in context menu', async () => {
        await page.goto(process.env.SP_URL);
        //select uploaded file
        //check for download button in action bar
        expect(1 == 1).toBe(true);
    });
})

afterEach(async () => {
    
});

afterAll(async () => {
    //delete uploaded file
    await page.close();
    await browser.close();
});