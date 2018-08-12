using OpenQA.Selenium;
using System;
using System.Threading;
using Toss.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    [CaptureSeleniumLogs]
    public abstract class ServerTestBase
        :  IClassFixture<AspNetSiteServerFixture>, IClassFixture<BrowserFixture>, IClassFixture<CosmosDBFixture>
    {
        protected readonly AspNetSiteServerFixture _serverFixture;
        protected readonly CosmosDBFixture cosmosDBFixture;
        private static readonly AsyncLocal<IWebDriver> _browser = new AsyncLocal<IWebDriver>();
        private static readonly AsyncLocal<ILogs> _logs = new AsyncLocal<ILogs>();
        private static readonly AsyncLocal<ITestOutputHelper> _output = new AsyncLocal<ITestOutputHelper>();

        public static IWebDriver Browser => _browser.Value;

        public static ILogs Logs => _logs.Value;
        public static ITestOutputHelper Output => _output.Value;
        public ServerTestBase(BrowserFixture browserFixture,
            AspNetSiteServerFixture serverFixture, 
            CosmosDBFixture cosmosDBFixture,
            ITestOutputHelper output)
           
        {
            _serverFixture = serverFixture;
            this.cosmosDBFixture = cosmosDBFixture;
            _browser.Value = browserFixture.Browser;
            _logs.Value = browserFixture.Logs;
            _output.Value = output;
        }

        public void Navigate(string relativeUrl, bool noReload = false)
        {
            var absoluteUrl = new Uri(_serverFixture.RootUri, relativeUrl);

            if (noReload)
            {
                var existingUrl = Browser.Url;
                if (string.Equals(existingUrl, absoluteUrl.AbsoluteUri, StringComparison.Ordinal))
                {
                    return;
                }
            }

            Browser.Navigate().GoToUrl(absoluteUrl);
        }

    }
}