using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Toss.Client;
using Toss.Server.Services;
using Toss.Tests.Infrastructure;

namespace Toss.Tests.E2E
{
    public class AspNetSiteServerFixture : IDisposable
    {
        public FakeEmailSender EmailSender { get; private set; }
        public Uri RootUri => _rootUriInitializer.Value;

        public IWebHost Host { get => _host; set => _host = value; }

        private IWebHost _host;
        private readonly Lazy<Uri> _rootUriInitializer;

        public AspNetSiteServerFixture()
        {
            _rootUriInitializer = new Lazy<Uri>(() =>
                new Uri(StartAndGetRootUri()));
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

        protected string StartAndGetRootUri()
        {
            Host = CreateWebHost();
            RunInBackgroundThread(Host.Start);
            EmailSender = Host.Services.GetService(typeof(IEmailSender)) as FakeEmailSender;
            return Host.ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses.Single();
        }

        public void Dispose()
        {
            // This can be null if creating the webhost throws, we don't want to throw here and hide
            // the original exception.
            Host?.StopAsync();
        }

        protected IWebHost CreateWebHost()
        {
            var solutionDir = FindClosestDirectoryContaining(
                          "Toss.sln",
                          Path.GetDirectoryName(typeof(Program).Assembly.Location));
            var sampleSitePath = Path.Combine(solutionDir, "Toss", typeof(Toss.Server.Program).Assembly.GetName().Name);
          
            return Toss.Server.Program.BuildWebHost(new[]
            {
                "--urls", "http://127.0.0.1:0",
                "--contentroot", sampleSitePath,
                "--environment", "development",
                "--databaseName",CosmosDBFixture.DatabaseName,
                "--e2eTest","true"
            });
        }
    }
}