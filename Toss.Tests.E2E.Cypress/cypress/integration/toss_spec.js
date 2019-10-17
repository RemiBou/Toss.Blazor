/// <reference types="Cypress" />

describe('Toss Full Test', function () {
    let polyfill;
    const uuid = Cypress._.random(0, 1e6)

    before(() => {
        const polyfillUrl = 'https://unpkg.com/whatwg-fetch@3.0.0/dist/fetch.umd.js';
        cy.request(polyfillUrl).then(response => {
            polyfill = response.body;
        });
    });
    Cypress.on('window:before:load', win => {
        delete win.fetch;
        win.eval(polyfill);
    });
    const SubscribeEmail = "tosstests" + uuid + "@yopmail.com";
    const SubscribePassword = "tossTests123456!!";
    const SubscribeLogin = "tosstests" + uuid;

    it('Full process', function () {
        cy.server();
        //used for listenning to register api call and getting the recirction url from the http headers
        cy.route('POST', '/api/account/register').as('register');
        cy.visit("/");

        disableCaptcha();

        //this could be long as ravendb is starting
        cy.get("#LinkLogin", { timeout: 20000 }).click();

        //register
        cy.get("#LinkRegister").click();
        cy.get("#NewEmail").type(SubscribeEmail);
        cy.get("#NewName").type(SubscribeLogin);
        cy.get("#NewPassword").type(SubscribePassword);
        cy.get("#NewConfirmPassword").type(SubscribePassword);
        cy.get("#BtnRegister").click();
        cy.wait('@register');
        cy.get('@register').then(function (xhr) {
            expect(xhr.status).to.eq(200);
            expect(xhr.response.headers['x-test-confirmationlink']).to.not.empty;
            cy.log("Redirect URL : " + xhr.response.headers['x-test-confirmationlink']);
            cy.visit(xhr.response.headers['x-test-confirmationlink']);
            disableCaptcha();
            //login
            cy.get("#UserName").type(SubscribeEmail);
            cy.get("#Password").type(SubscribePassword);
            cy.get("#BtnLogin").click();
            //publish toss
            cy.get("#LinkNewToss").click();
            var newTossContent = "lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsum #test";
            cy.get("#TxtNewToss").type(newTossContent);
            cy.get("#BtnNewToss").click();
        });

        // //log in

        // _webDriveWaitDefault.Until(b => b.Url.EndsWith("/"));

        // //publish toss


        // //add new toss x 2
        // Browser.FindElement(By.Id("LinkNewToss")).Click();
        // _webDriveWaitDefault.Until(b => b.FindElement(By.Id("TxtNewToss")).Displayed);
        // Browser.FindElement(By.Id("TxtNewToss")).SendKeys(@" lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum  lorem ipsumlorem ipsum lorem ipsum #toto");
        // Browser.FindElement(By.Id("BtnNewToss")).Click();
        // _webDriveWaitDefault.Until(b => b.Url.EndsWith("/"));

        // //add new hashtag
        // Browser.FindElement(By.Id("TxtAddHashTag")).SendKeys(@"test");
        // Browser.FindElement(By.Id("TxtAddHashTag")).SendKeys(Environment.NewLine);
        // Browser.FindElement(By.Id("BtnAddHashTag")).Click();
        // _webDriveWaitDefault.Until(b => b.FindElements(By.CssSelector(".tag-link")).Any());

        // //filter on hashtag
        // Browser.FindElement(By.CssSelector(".tag-link")).Click();

        // // read first toss
        // _webDriveWaitDefault.Until(b => b.FindElements(By.CssSelector(".toss-preview")).Any());
        // Browser.FindElement(By.CssSelector(".toss-preview")).Click();
        // _webDriveWaitDefault.Until(b => b.FindElement(By.CssSelector(".toss-detail .toss-content")).Text == newTossContent);

        // //sign out
        // Browser.FindElement(By.Id("LinkAccount")).Click();
        // _webDriveWaitDefault.Until(b => b.Url.EndsWith("/account"));
        // ScrollToView(By.Id("BtnLogout"));
        // Browser.FindElement(By.Id("BtnLogout")).Click();
        // _webDriveWaitDefault.Until(b => b.Url.EndsWith("/login"));
    })
})

function disableCaptcha() {
    cy.window()
        .then(win => {
            win.runCaptcha = new win.Function(['action'], 'return Promise.resolve(action)');
        });
}
