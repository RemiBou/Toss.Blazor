using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Toss.Server
{
    public class Program
    {
        /*
        Needed by E2E tests
         */
        public static IWebHostBuilder WebHostBuilder { get; private set; }
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IHostBuilder BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(new ConfigurationBuilder()
                        .AddCommandLine(args)
                        .AddUserSecrets<Startup>()
                        .AddJsonFile("/run/secrets/tossserver")
                        .AddEnvironmentVariables()
                        .Build())
                    .UseStartup<Startup>();
                });
    }
}
