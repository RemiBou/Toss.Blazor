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
            Navigate("/", noReload: true);
            WaitUntilLoaded();
            _webDriveWaitDefault = new WebDriverWait(Browser, TimeSpan.FromSeconds(DefaultWaitSecondsForPageChange));
        }
        /// <summary>
        /// The purpose of this test is the most important features of toss : subscribe, validate account, post content, read content
        /// Forget password, reset password, logout.
        /// As more feature will be added if tey are important enough tey'll be added to this test
        /// It's designed as one test because E2E test are hard to initialize so I prefer to see it as a signle user session trying everything.
        /// 
        /// </summary>
        [Fact]
        public void FullE2eTest()
        {
            Navigate("/");
            Assert.Equal("Toss", Browser.Title);
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
