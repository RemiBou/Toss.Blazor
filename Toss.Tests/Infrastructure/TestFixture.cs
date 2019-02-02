using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Identity;
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

        public static ClaimsPrincipal ClaimPrincipal { get; set; }
        public static Mock<HttpContext> HttpContextMock { get; private set; }
        public static Mock<HttpRequest> HttpRequestMock { get; private set; }
        public static Mock<IHttpContextAccessor> HttpContextAccessor { get; set; }
        public static Mock<IActionContextAccessor> ActionContextAccessor { get; set; }
        public static RavenDBTestDriver TestDriver { get; }

        static TestFixture()
        {

            var dict = new Dictionary<string, string>
            {
                 { "GoogleClientId", ""},
                 { "GoogleClientSecret", ""},
                 { "MailJetApiKey", ""},
                 { "MailJetApiSecret", ""},
                 { "MailJetSender", ""},
                 { "RavenDBEndpoint", "http://127.0.0.1:8081"},
                 { "StripeSecretKey", ""},
                 { "test", "true"},
                 { "dataBaseName", DataBaseName}
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .AddEnvironmentVariables()
                .Build();

            var startup = new Startup(config);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);

            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            HttpContextMock = new Mock<HttpContext>();
            DefaultConnectionInfo connectionInfo = new DefaultConnectionInfo(new FeatureCollection());
            connectionInfo.RemoteIpAddress = new System.Net.IPAddress(0x2414188f);
            HttpContextMock.SetupGet(c => c.Connection).Returns(connectionInfo);
            HttpContextAccessor
               .SetupGet(h => h.HttpContext)
               .Returns(() => HttpContextMock.Object);
            HttpRequestMock = new Mock<HttpRequest>();
            HttpContextMock.SetupGet(m => m.Request).Returns(HttpRequestMock.Object);
            HttpRequestMock.SetupGet(r => r.Host).Returns(new HostString("localhost"));


            ActionContextAccessor = new Mock<IActionContextAccessor>();
            ActionContextAccessor.SetupGet(a => a.ActionContext).Returns(new ActionContext(HttpContextMock.Object, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor()));
            TestDriver = new RavenDBTestDriver();

            TestDriver.ConfigureService();
            services.AddSingleton<IDocumentStore>(TestDriver.GetDocumentStore());
            services
                .AddRavenDbAsyncSession(TestDriver.GetDocumentStore())
                .AddRavenDbIdentity<ApplicationUser>();
            services.AddSingleton(HttpContextAccessor.Object);
            services.AddSingleton(ActionContextAccessor.Object);
            services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            _provider = services.BuildServiceProvider();

        }

        public async static Task CreateTestUser()
        {
            await CreateNewUserIfNotExists(TestFixture.UserName);
        }

        public static async Task CreateNewUserIfNotExists(string userName)
        {
            var userManager = _provider.GetService<UserManager<ApplicationUser>>();
            var existing = await userManager.FindByNameAsync(userName);
            if (existing == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = userName + "@yopmail.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user);
                TestFixture.TestDriver.WaitIndexing();

                existing = user;
            }
            ClaimPrincipal = new ClaimsPrincipal(
                      new ClaimsIdentity(new Claim[]
                         {
                                    new Claim(ClaimTypes.Name, userName),
                                    new Claim(ClaimTypes.NameIdentifier, existing.Id)
                         },
                      "Basic"));
            HttpContextMock.SetupGet(m => m.User).Returns(ClaimPrincipal);

        }

        public static void SetControllerContext(Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContextAccessor.Object.HttpContext
            };
        }

        public static void SetControllerContext(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContextAccessor.Object.HttpContext
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
