using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    [CaptureSeleniumLogs]
    public class BrowserTestBase : IClassFixture<BrowserFixture>
    {
        private static readonly AsyncLocal<IWebDriver> _browser = new AsyncLocal<IWebDriver>();
        private static readonly AsyncLocal<ILogs> _logs = new AsyncLocal<ILogs>();
        private static readonly AsyncLocal<ITestOutputHelper> _output = new AsyncLocal<ITestOutputHelper>();

        public static IWebDriver Browser => _browser.Value;

        public static ILogs Logs => _logs.Value;

        public static ITestOutputHelper Output => _output.Value;

        public BrowserTestBase(BrowserFixture browserFixture, ITestOutputHelper output)
        {
            _browser.Value = browserFixture.Browser;
            _logs.Value = browserFixture.Logs;
            _output.Value = output;
        }
    }
}