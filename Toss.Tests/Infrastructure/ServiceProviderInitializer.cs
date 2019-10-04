using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Toss.Server;
using Toss.Server.Models;

namespace Toss.Tests.Infrastructure
{
    public class ServiceProviderInitializer
    {
        private ServiceProvider _provider;

        public ClaimsPrincipal ClaimPrincipal { get; set; }
        public Mock<HttpContext> HttpContextMock { get; private set; }
        public Mock<HttpRequest> HttpRequestMock { get; private set; }
        public Mock<IHttpContextAccessor> HttpContextAccessor { get; set; }
        public Mock<IActionContextAccessor> ActionContextAccessor { get; set; }
        public string UserName
        {
            get
            {
                return ClaimPrincipal.Identity.Name;
            }
        }

        public void BuildServiceProvider(Raven.Client.Documents.IDocumentStore documentStore)
        {

            var dict = new Dictionary<string, string>
            {
                 { "GoogleClientId", ""},
                 { "GoogleClientSecret", ""},
                 { "MailJetApiKey", ""},
                 { "MailJetApiSecret", ""},
                 { "MailJetSender", ""},
                 { "RavenDBEndpoint",documentStore.Urls[0]},
                 {"RavenDBDataBase",documentStore.Database },
                 { "StripeSecretKey", ""},
                 { "test", "true"},
                 { "dataBaseName", ""}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .AddEnvironmentVariables()
                .Build();

            var startup = new Startup(config);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);

            InitMockHttpServices(services);
            InitLogger(services);

            _provider = services.BuildServiceProvider();

        }

        private static void InitLogger(ServiceCollection services)
        {
            services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
        }

        private void InitMockHttpServices(ServiceCollection services)
        {
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            HttpContextMock = new Mock<HttpContext>();
            var connectionInfoMock = new Mock<ConnectionInfo>();
            connectionInfoMock.SetupGet(c => c.RemoteIpAddress).Returns(new System.Net.IPAddress(0x2414188f));
            ConnectionInfo connectionInfo = connectionInfoMock.Object;

            HttpContextMock.SetupGet(c => c.Connection).Returns(connectionInfo);
            HttpContextMock.SetupGet(c => c.Items).Returns(new Dictionary<object, object>());

            HttpContextMock.SetupGet(c => c.Features).Returns(new FeatureCollection());
            HttpContextAccessor
               .SetupGet(h => h.HttpContext)
               .Returns(() => HttpContextMock.Object);
            HttpRequestMock = new Mock<HttpRequest>();
            HttpContextMock.SetupGet(m => m.Request).Returns(HttpRequestMock.Object);
            HttpRequestMock.SetupGet(r => r.Host).Returns(new HostString("localhost"));


            ActionContextAccessor = new Mock<IActionContextAccessor>();
            ActionContextAccessor.SetupGet(a => a.ActionContext).Returns(new ActionContext(HttpContextMock.Object, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor()));

            services.AddSingleton(HttpContextAccessor.Object);
            services.AddSingleton(ActionContextAccessor.Object);
        }



        public void SetControllerContext(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContextAccessor.Object.HttpContext
            };
        }

        public T GetInstance<T>()
        {
            T result = _provider.GetRequiredService<T>();
            if (result is ControllerBase controllerBase)
            {
                SetControllerContext(controllerBase);
            }
            return result;

        }

        public void ChangeCurrentUser(ApplicationUser user)
        {
            ClaimPrincipal = new ClaimsPrincipal(
                      new ClaimsIdentity(new Claim[]
                         {
                                    new Claim(ClaimTypes.Name, user.UserName),
                                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                         },
                      "Basic"));
            HttpContextMock.SetupGet(m => m.User).Returns(ClaimPrincipal);
        }
    }
}
