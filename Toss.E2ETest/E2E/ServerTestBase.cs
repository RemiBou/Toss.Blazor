using System;
using Xunit;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    public abstract class ServerTestBase<TServerFixture>
        : BrowserTestBase, IClassFixture<TServerFixture>
        where TServerFixture : ServerFixture
    {
        private readonly TServerFixture _serverFixture;

        public ServerTestBase(BrowserFixture browserFixture, TServerFixture serverFixture, ITestOutputHelper output)
            : base(browserFixture, output)
        {
            _serverFixture = serverFixture;
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