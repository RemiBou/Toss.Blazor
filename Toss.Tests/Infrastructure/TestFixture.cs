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
using Raven.Client.Documents.Conventions;
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
            var testDriver = new RavenDBTestDriver();

            testDriver.ConfigureService();
            services.AddSingleton(testDriver.GetDocumentStore());
            services.AddSingleton(HttpContextAccessor.Object);
            services.AddSingleton(ActionContextAccessor.Object);
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
                Email = userName + "@yopmail.com",
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
