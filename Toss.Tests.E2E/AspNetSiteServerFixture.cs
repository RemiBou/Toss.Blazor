using System;
using System.Collections.Generic;
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

        public IWebHost Host { get; set; }

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
            var sampleSitePath = Path.Combine(solutionDir, typeof(Toss.Server.Program).Assembly.GetName().Name);
            var config = new Dictionary<string, string>
            {
                 { "GoogleClientId", "AAA"},
                 { "GoogleClientSecret", "AAA"},
                 { "MailJetApiKey", ""},
                 { "MailJetApiSecret", ""},
                 { "MailJetSender", ""},
                 { "CosmosDBEndpoint", "https://localhost:8081"},
                 { "CosmosDBKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="},
                 { "StripeSecretKey", ""},
                 { "test", "true"},
                 { "dataBaseName", CosmosDBFixture.DatabaseName}
            };
            //we set the config in env variables because those value
            //can't be in appsettings as it'll override the secrets
            foreach (var key in config)
            {
                //CI env might set some values like CosmosDB endpoint url
                if (Environment.GetEnvironmentVariable(key.Key) == null)
                {
                    Environment.SetEnvironmentVariable(key.Key, key.Value);
                }
            }
            return Toss.Server.Program.BuildWebHost(new[]
            {
                "--urls", "http://127.0.0.1:0",
                "--contentroot", sampleSitePath,
                "--environment", "development"
            });
        }
    }
}