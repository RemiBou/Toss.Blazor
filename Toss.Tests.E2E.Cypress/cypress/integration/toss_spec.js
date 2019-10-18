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
        cy.route('POST', '/api/account/login').as('login');
        cy.route('POST', '/api/toss/create').as('create');
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
            cy.get("#UserName", { timeout: 20000 }).type(SubscribeEmail);
            cy.get("#Password").type(SubscribePassword);
            cy.get("#BtnLogin").click();
            cy.wait('@login');
            //publish toss
            cy.get("#LinkNewToss").click();
            var newTossContent = "lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsum #test";
            cy.get("#TxtNewToss").type(newTossContent);
            cy.get("#BtnNewToss").click();
            cy.wait('@create');

            //publish toss x2
            cy.get("#LinkNewToss").click();
            var newTossContent2 = " lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum  lorem ipsumlorem ipsum lorem ipsum #toto";
            cy.get("#TxtNewToss").type(newTossContent2);
            cy.get("#BtnNewToss").click();
            cy.wait('@create');


            //add new hashtag
            cy.get("#TxtAddHashTag").type("test");
            cy.get("#TxtAddHashTag").type("{enter}");
            cy.get("#BtnAddHashTag").click();
            cy.get(".toss-preview").first().click();
            cy.get(".toss-detail .toss-content").should("contain", newTossContent);

            // logout
            cy.get("#LinkAccount").click();

            cy.get("#BtnLogout").click();
            cy.url().should("eq", Cypress.config().baseUrl + "/");
        });
    })
})

function disableCaptcha() {
    cy.window()
        .then(win => {
            win.runCaptcha = new win.Function(['action'], 'return Promise.resolve(action)');
        });
}
