using System;
using Microsoft.AspNetCore.Hosting;

namespace Toss.Tests.E2E
{
    public class AspNetSiteServerFixture : WebHostServerFixture
    {
        public delegate IWebHost BuildWebHost(string[] args);

        public BuildWebHost BuildWebHostMethod { get; set; }
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
                "--environment", "development",
            });
        }
    }
}