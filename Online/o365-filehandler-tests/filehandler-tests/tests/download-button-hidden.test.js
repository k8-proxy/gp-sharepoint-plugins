const sphelper = require('../utils/sp-helper');
const fileToUpload = 'resources/cleaned_files/TestAutoCurImage.jpg';

beforeAll(async () => {
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
        //select uploaded file
        //check for download button in context menu
        expect(1 == 1).toBe(true);
    });
})

afterEach(async () => {
    
});

afterAll(async () => {
    //delete uploaded file
});