using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private static ServiceProvider _provider;
        //only mock we need :)
        private static Mock<IHttpContextAccessor> _httpContextAccessor;

        private static DefaultHttpContext HttpContext;
        public static ClaimsPrincipal ClaimPrincipal { get; set; }

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
            await ChangeCurrentUser(TestFixture.UserName);
        }

        public static async Task ChangeCurrentUser(string userName)
        {
            var userManager = _provider.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = new ApplicationUser()
            {
                UserName = userName,
                Email = userName+"@yopmail.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user);
            ClaimPrincipal = new ClaimsPrincipal(
                      new ClaimsIdentity(new Claim[]
                         {
                                    new Claim(ClaimTypes.Name, userName),
                                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                         },
                      "Basic"));
            HttpContext = new DefaultHttpContext()
            {
                User = ClaimPrincipal
            };
            _httpContextAccessor
              .SetupGet(h => h.HttpContext)
              .Returns(() => HttpContext);
        }

        public static void SetControllerContext(Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextAccessor.Object.HttpContext
            };
        }

        public static void SetControllerContext(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext  = _httpContextAccessor.Object.HttpContext
            };
        }

        public static T GetInstance<T>()
        {
            T result = _provider.GetRequiredService<T>();
            ControllerBase controllerBase = result as ControllerBase;
            if (controllerBase != null)
            {
                SetControllerContext(controllerBase);
            }
            Controller controller = result as Controller;
            if (controller != null)
            {
                SetControllerContext(controller);
            }
            return result;

        }
    }
}
