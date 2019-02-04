using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Identity;
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
            services.AddSingleton(documentStore);
            services
                .AddRavenDbAsyncSession(documentStore)
                .AddRavenDbIdentity<ApplicationUser>();
            InitMockHttpServices(services);
            services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            _provider = services.BuildServiceProvider();

        }

        private void InitMockHttpServices(ServiceCollection services)
        {
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

            services.AddSingleton(HttpContextAccessor.Object);
            services.AddSingleton(ActionContextAccessor.Object);
        }



        public void SetControllerContext(Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContextAccessor.Object.HttpContext
            };
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
