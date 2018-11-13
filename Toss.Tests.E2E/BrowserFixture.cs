using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit.Abstractions;

namespace Toss.Tests.E2E
{
    public class BrowserFixture : IDisposable
    {
        public IWebDriver Browser { get; }

        public ILogs Logs { get; }

        public ITestOutputHelper Output { get; set; }

        public BrowserFixture()
        {
            var opts = new ChromeOptions();

            // Comment this out if you want to watch or interact with the browser (e.g., for debugging)
            //opts.AddArgument("--headless");

            // Log errors
            opts.SetLoggingPreference(LogType.Browser, LogLevel.All);

            // On Windows/Linux, we don't need to set opts.BinaryLocation
            // But for Travis Mac builds we do
            var binaryLocation = Environment.GetEnvironmentVariable("ChromeWebDriver");
            if (!string.IsNullOrEmpty(binaryLocation))
            {
                opts.BinaryLocation = binaryLocation;
                Console.WriteLine($"Set {nameof(ChromeOptions)}.{nameof(opts.BinaryLocation)} to {binaryLocation}");
            }

            var driver = new ChromeDriver(opts);
            Browser = driver;
        }

        public void Dispose()
        {
            Browser.Dispose();
        }
    }
}