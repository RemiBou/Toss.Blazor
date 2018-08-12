using System;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using Xunit.Sdk;

namespace Toss.Tests.E2E
{
    public class CaptureSeleniumLogsAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            if (!typeof(ServerTestBase).IsAssignableFrom(methodUnderTest.DeclaringType))
            {
                throw new InvalidOperationException("This should only be used with BrowserTestBase");
            }
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var browser = ServerTestBase.Browser;
            var logs = ServerTestBase.Logs;
            var output = ServerTestBase.Output;

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
}