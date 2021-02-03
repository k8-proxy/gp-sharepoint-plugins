const {chromium} = require('playwright');
const expect = require('expect');

let browser;
let page;
let sharepointUrl;

beforeAll(async () => {
  browser = await chromium.launch({headless:false});
  sharepointUrl = process.env.SP_URL
  console.log(`sharepointUrl:${sharepointUrl}`);
});

beforeEach(async () => {
  page = await browser.newPage();
});

it('PDF Rebuild', async () => {
  //upload a pdf file
  //download pdf file from action bar
  //verify file rebuild
  //download pdf file from context menu
  //verify file rebuild
});

it('JPG Rebuild', async () => {
  //upload a JPG file
  //download JPG file from action bar
  //verify file rebuild
  //download JPG file from context menu
  //verify file rebuild
});

afterEach(async () => {
  await page.close();
});

afterAll(async () => {
  await browser.close();
});
