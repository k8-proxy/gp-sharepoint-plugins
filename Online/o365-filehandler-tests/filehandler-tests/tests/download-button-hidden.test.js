const {chromium} = require('playwright');
const expect = require('expect');
const sphelper = require('../utils/sp-helper');
const fileToUpload = 'resources/cleaned_files/TestAutoCurImage.jpg';
let browser;
let context;
let page;
let sharepointUrl;

beforeAll(async () => {
  browser = await chromium.launch({headless:false});
  context = await browser.newContext();

});


beforeEach(async () => {
  page = await context.newPage();
  await sphelper.navigateToSP(page);
  await sphelper.login(page);
  await sphelper.uploadFile(page,fileToUpload)  
});



it('Download button is hidden in action bar', async () => {
  //Select a file
    
  //verify action bar does not contain default download button

});

// it('Download button is hidden in context menu', async () => {
//   //Select a file
//   //verify context menu does not contain default download button
// });

afterEach(async () => {
//  delete the uploaded file
//  await page.close();
});

// afterAll(async () => {
//   await browser.close();
//});