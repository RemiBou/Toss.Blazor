using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config
                        .AddCommandLine(args)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddJsonFile("/run/secrets/tossserver", true)//will be used for mounting secrets on docker                        
                        .AddUserSecrets<Startup>()
                        .AddEnvironmentVariables();

                })
                .UseKestrel(k => k.AddServerHeader = false)
                .UseStartup<Startup>();

    }
}
