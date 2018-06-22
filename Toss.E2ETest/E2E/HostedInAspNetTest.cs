using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    public class HostedInAspNetTest : ServerTestBase<AspNetSiteServerFixture>
    {
        private readonly WebDriverWait _webDriveWaitDefault;
        private const int DefaultWaitSecondsForPageChange = 2;
        public HostedInAspNetTest(
            BrowserFixture browserFixture,
            AspNetSiteServerFixture serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
            serverFixture.BuildWebHostMethod = Toss.Server.Program.BuildWebHost;
            serverFixture.Environment = AspNetEnvironment.Development;
            Navigate("/", noReload: true);
            WaitUntilLoaded();
            _webDriveWaitDefault = new WebDriverWait(Browser, TimeSpan.FromSeconds(DefaultWaitSecondsForPageChange));
        }

  //      [Fact]
        public void HasTitle()
        {
            Assert.Equal("TOSS", Browser.Title);
        }

 //       [Fact]
        public void when_accessing_accountpage_and_not_logged_redirect_to_login()
        {
            Navigate("/account");
            
            _webDriveWaitDefault.Until(driver => driver.Url.EndsWith("/login"));
        }

//        [Fact]
        public void when_register_ok_empty_form_and_cannot_log()
        {
            Navigate("/login");

            _webDriveWaitDefault.Until(driver => driver.FindElements(By.Id("NewEmail")).Any());
            Browser.FindElement(By.Id("NewEmail")).SendKeys("toto@yopmail.com");
            Browser.FindElement(By.Id("NewName")).SendKeys("tototo");
            Browser.FindElement(By.Id("NewPassword")).SendKeys("056187Aa!");
            Browser.FindElement(By.Id("NewConfirmPassword")).SendKeys("056187Aa!");
            Browser.FindElement(By.Id("BtnRegister")).Click();
            _webDriveWaitDefault.Until(driver => driver.FindElement(By.Id("NewEmail")).GetAttribute("value") == "");
           
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
