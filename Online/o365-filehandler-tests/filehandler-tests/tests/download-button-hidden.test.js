const {chromium} = require('playwright');
const expect = require('expect');

let browser;
let page;
let sharepointUrl;

beforeAll(async () => {
  browser = await chromium.launch({headless:false});
  sharepointUrl = process.env.SP_URL
  console.log(`sharepointUrl:${sharepointUrl}`);
  console.log(`SP_SITE:${process.env.SP_SITE}`);
  console.log(`SP_USER:${process.env.SP_USER}`);
  console.log(`SP_USER_PWD:${process.env.SP_USER_PWD}`);
  //Upload some file
});

beforeEach(async () => {
  page = await browser.newPage();
});

it('Download button is hidden in action bar', async () => {
  //Select a file
  //verify action bar does not contain default download button
});

it('Download button is hidden in context menu', async () => {
  //Select a file
  //verify context menu does not contain default download button
});

afterEach(async () => {
  await page.close();
});

afterAll(async () => {
  await browser.close();
});
