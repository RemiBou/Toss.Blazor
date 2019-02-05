﻿using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    public class E2ETest : ServerTestBase
    {
        private readonly WebDriverWait _webDriveWaitDefault;
        private const int DefaultWaitSecondsForPageChange = 20;
        private const string SubscribeEmail = "toss-unittests@yopmail.com";
        private const string SubscribePassword = "tossUnittests123456!!";
        private const string SubscribeLogin = "tossunittests";

        public E2ETest(
            BrowserFixture browserFixture,
            AspNetSiteServerFixture serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
            Navigate("/", noReload: true);
            WaitUntilLoaded();
            _webDriveWaitDefault = new WebDriverWait(Browser, TimeSpan.FromSeconds(DefaultWaitSecondsForPageChange));
        }
        /// <summary>
        /// The purpose of this test is the most important features of toss 
        /// As more feature will be added if tey are important enough tey'll be added to this test
        /// It's designed as one test because E2E test are hard to initialize so I prefer to see it as a signle user session trying everything.
        /// 
        /// </summary>
        [Fact]
        public void FullE2eTest()
        {

            Browser.Manage().Window.FullScreen();
            Navigate("/login");
            DisableRecaptcha();
            Assert.Equal("TOSS", Browser.Title);
            //load and redirect to /login
            _webDriveWaitDefault.Until(b => b.FindElement(By.Id("NewEmail")) != null);

            //subscribe
            Browser.FindElement(By.Id("NewEmail")).SendKeys(SubscribeEmail);
            Browser.FindElement(By.Id("NewName")).SendKeys(SubscribeLogin);
            Browser.FindElement(By.Id("NewPassword")).SendKeys(SubscribePassword);
            Browser.FindElement(By.Id("NewConfirmPassword")).SendKeys(SubscribePassword);
            Browser.FindElement(By.Id("BtnRegister")).Click();
            _webDriveWaitDefault.Until(b => b.FindElement(By.Id("NewEmail")).GetAttribute("value") == "");

            //validate subscription
            var confirmationLink = _serverFixture.EmailSender.GetConfirmationLink(SubscribeEmail);
            Browser.Navigate().GoToUrl(confirmationLink);
            DisableRecaptcha();
            _webDriveWaitDefault.Until(b => b.Url.EndsWith("/login"));

            //log in
            Browser.FindElement(By.Id("UserName")).SendKeys(SubscribeLogin);
            Browser.FindElement(By.Id("Password")).SendKeys(SubscribePassword);
            Browser.FindElement(By.Id("BtnLogin")).Click();
            _webDriveWaitDefault.Until(b => b.Url.EndsWith("/"));

            //publish toss
            Browser.FindElement(By.Id("BtnOpenNewToss")).Click();
            _webDriveWaitDefault.Until(b => b.FindElement(By.Id("TxtNewToss")).Displayed);
            string newTossContent = @"lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum lorem ipsum #test";
            Browser.FindElement(By.Id("TxtNewToss")).SendKeys(newTossContent);
            Browser.FindElement(By.Id("BtnNewToss")).Click();
            _webDriveWaitDefault.Until(b => !b.FindElements(By.CssSelector(".modal-backdrop")).Any());

            //add new toss x 2
            Browser.FindElement(By.Id("BtnOpenNewToss")).Click();
            _webDriveWaitDefault.Until(b => b.FindElement(By.Id("TxtNewToss")).Displayed);
            Browser.FindElement(By.Id("TxtNewToss")).SendKeys(@" lorem ipsum lorem ipsumlorem ipsum lorem ipsumlorem ipsum  lorem ipsumlorem ipsum lorem ipsum #toto");
            Browser.FindElement(By.Id("BtnNewToss")).Click();
            _webDriveWaitDefault.Until(b => !b.FindElements(By.CssSelector(".modal-backdrop")).Any());

            //add new hashtag
            Browser.FindElement(By.Id("TxtAddHashTag")).SendKeys(@"test");
            Browser.FindElement(By.Id("BtnAddHashTag")).Click();
            _webDriveWaitDefault.Until(b => b.FindElements(By.CssSelector(".tag-link")).Any());

            //filter on hashtag
            Browser.FindElement(By.CssSelector(".tag-link")).Click();
            _webDriveWaitDefault.Until(b => b.FindElement(By.CssSelector(".toss .card-text")).Text == newTossContent);

            //sign out

            Browser.FindElement(By.Id("LinkLogout")).Click();
            _webDriveWaitDefault.Until(b => b.Url.EndsWith("/login"));
            //reset password
            //click reset link
            //do reset password
            //connect
        }

        private static void DisableRecaptcha()
        {
            if (Browser is IJavaScriptExecutor)
            {
                //in E2E test we disable getting the token from recaptcha
                ((IJavaScriptExecutor)Browser).ExecuteScript("runCaptcha = function(actionName) { return Promise.resolve('test'); }");
            }
        }

        private void WaitUntilLoaded()
        {
            new WebDriverWait(Browser, TimeSpan.FromSeconds(30)).Until(
                 driver => !driver.FindElement(By.TagName("app")).Text.Contains("Loading...")
                );
        }
    }

    // This has to use BeforeAfterTestAttribute because running the log capture
    // in the BrowserFixture.Dispose method is too late, and we can't add logging
    // to the test.
}
