using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Toss.Server;
using Toss.Server.Models;

namespace Toss.Tests.Infrastructure
{
    public class TestFixture
    {
        public const string DataBaseName = "Tests";
        public const string UserName = "username";
        private static readonly IServiceScopeFactory _scopeFactory;
        private static ServiceProvider _provider;
        private static Mock<IHttpContextAccessor> _httpContextAccessor;

        static TestFixture()
        {
            
            var dict = new Dictionary<string, string>
            {
                 { "GoogleClientId", ""},
                 { "GoogleClientSecret", ""},
                 { "MailJetApiKey", ""},
                 { "MailJetApiSecret", ""},
                 { "MailJetSender", ""},
                 { "CosmosDBEndpoint", "https://localhost:8081"},
                 { "CosmosDBKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="},
                 { "StripeSecretKey", ""},
                {"test","true" },
                {"dataBaseName",DataBaseName }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var startup = new Startup(config);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
          
            services.AddSingleton(_httpContextAccessor.Object);
            services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            _provider = services.BuildServiceProvider();
            
        }

        public async static Task CreateTestUser()
        {
            var userManager =  _provider.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = new ApplicationUser()
            {
                UserName = UserName,
                Email = "test@yopmail.com"
            };
            await userManager.CreateAsync(user);
            var claimPrincipal = new ClaimsPrincipal(
                      new ClaimsIdentity(new Claim[]
                         {
                                    new Claim(ClaimTypes.Name, UserName)
                         },
                      "someAuthTypeName"));
            (claimPrincipal.Identity as ClaimsIdentity).AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            _httpContextAccessor
              .SetupGet(h => h.HttpContext)
              .Returns(() =>
              new DefaultHttpContext()
              {
                  User = claimPrincipal
                  
              });
        }


        public static T GetInstance<T>()
        {
            return _provider.GetRequiredService<T>();
        }
    }
}
