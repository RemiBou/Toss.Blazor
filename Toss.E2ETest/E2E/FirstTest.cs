using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Toss.E2ETest
{
    public class HostedInAspNetTest : ServerTestBase<AspNetSiteServerFixture>
    {
        private WebDriverWait _webDriveWaitDefault;
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
    public abstract class ServerFixture : IDisposable
    {
        public Uri RootUri => _rootUriInitializer.Value;

        private readonly Lazy<Uri> _rootUriInitializer;

        public ServerFixture()
        {
            _rootUriInitializer = new Lazy<Uri>(() =>
                new Uri(StartAndGetRootUri()));
        }

        public abstract void Dispose();

        protected abstract string StartAndGetRootUri();

        protected static string FindSolutionDir()
        {
            return FindClosestDirectoryContaining(
                "Toss.sln",
                Path.GetDirectoryName(typeof(ServerFixture).Assembly.Location));
        }

        protected static string FindSampleOrTestSitePath(string projectName)
        {
            var solutionDir = FindSolutionDir();
            return Path.Combine(solutionDir, "Toss", projectName);
        }

        private static string FindClosestDirectoryContaining(
            string filename,
            string startDirectory)
        {
            var dir = startDirectory;
            while (true)
            {
                if (File.Exists(Path.Combine(dir, filename)))
                {
                    return dir;
                }

                dir = Directory.GetParent(dir)?.FullName;
                if (string.IsNullOrEmpty(dir))
                {
                    throw new FileNotFoundException(
                        $"Could not locate a file called '{filename}' in " +
                        $"directory '{startDirectory}' or any parent directory.");
                }
            }
        }

        protected static void RunInBackgroundThread(Action action)
        {
            var isDone = new ManualResetEvent(false);

            new Thread(() =>
            {
                action();
                isDone.Set();
            }).Start();

            isDone.WaitOne();
        }
    }
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
            var binaryLocation = Environment.GetEnvironmentVariable("TEST_CHROME_BINARY");
            if (!string.IsNullOrEmpty(binaryLocation))
            {
                opts.BinaryLocation = binaryLocation;
                Console.WriteLine($"Set {nameof(ChromeOptions)}.{nameof(opts.BinaryLocation)} to {binaryLocation}");
            }

            try
            {
                var driver = new RemoteWebDriver(opts);
                Browser = driver;
                Logs = new RemoteLogs(driver);
            }
            catch (WebDriverException ex)
            {
                var message =
                    "Failed to connect to the web driver. Please see the readme and follow the instructions to install selenium." +
                    "Remember to start the web driver with `selenium-standalone start` before running the end-to-end tests.";
                throw new InvalidOperationException(message, ex);
            }
        }

        public void Dispose()
        {
            Browser.Dispose();
        }
    }
    // This has to use BeforeAfterTestAttribute because running the log capture
    // in the BrowserFixture.Dispose method is too late, and we can't add logging
    // to the test.
    public class CaptureSeleniumLogsAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            if (!typeof(BrowserTestBase).IsAssignableFrom(methodUnderTest.DeclaringType))
            {
                throw new InvalidOperationException("This should only be used with BrowserTestBase");
            }
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var browser = BrowserTestBase.Browser;
            var logs = BrowserTestBase.Logs;
            var output = BrowserTestBase.Output;

            // Put browser logs first, the test UI will truncate output after a certain length
            // and the browser logs will include exceptions thrown by js in the browser.
            foreach (var kind in logs.AvailableLogTypes.OrderBy(k => k == LogType.Browser ? 0 : 1))
            {
                output.WriteLine($"{kind} Logs from Selenium:");

                var entries = logs.GetLog(kind);
                foreach (LogEntry entry in entries)
                {
                    output.WriteLine($"[{entry.Timestamp}] - {entry.Level} - {entry.Message}");
                }

                output.WriteLine("");
                output.WriteLine("");
            }
        }
    }
    public class AspNetSiteServerFixture : WebHostServerFixture
    {
        public delegate IWebHost BuildWebHost(string[] args);

        public BuildWebHost BuildWebHostMethod { get; set; }

        public AspNetEnvironment Environment { get; set; } = AspNetEnvironment.Production;

        protected override IWebHost CreateWebHost()
        {
            if (BuildWebHostMethod == null)
            {
                throw new InvalidOperationException(
                    $"No value was provided for {nameof(BuildWebHostMethod)}");
            }

            var sampleSitePath = FindSampleOrTestSitePath(
                BuildWebHostMethod.Method.DeclaringType.Assembly.GetName().Name);

            return BuildWebHostMethod(new[]
            {
                "--urls", "http://127.0.0.1:0",
                "--contentroot", sampleSitePath,
                "--environment", Environment.ToString(),
            });
        }
    }
    public abstract class WebHostServerFixture : ServerFixture
    {
        private IWebHost _host;

        protected override string StartAndGetRootUri()
        {
            _host = CreateWebHost();
            RunInBackgroundThread(_host.Start);
            return _host.ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses.Single();
        }

        public override void Dispose()
        {
            // This can be null if creating the webhost throws, we don't want to throw here and hide
            // the original exception.
            _host?.StopAsync();
        }

        protected abstract IWebHost CreateWebHost();
    }
    public enum AspNetEnvironment
    {
        Development,
        Production
    }
}
