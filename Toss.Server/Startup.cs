using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Blazor.Server;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Toss.Server.Data;
using MediatR;
using Toss.Server.Services;
using Toss.Shared.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Toss.Server.Models;
using Toss.Server.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Rewrite;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http;
using Raven.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Toss.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            IDocumentStore store = new DocumentStore()
            {
                Urls = new[] { Configuration.GetValue<string>("RavenDBEndpoint") },
                Database = "Toss"
            }.Initialize();
            services.AddSingleton(store);
            services.AddScoped<IAsyncDocumentSession>((s) => s.GetService<IDocumentStore>().OpenAsyncSession());

            services
                .AddRavenDbIdentity<ApplicationUser>();

            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                            .ActionContext;
                return new UrlHelper(actionContext);
            });
            // Add application services.
            if (Configuration.GetValue<string>("test") == null)
            {
                services.AddSingleton<ICaptchaValidator>(s => new CaptchaValidator(
                    Configuration["GoogleCaptchaSecret"],
                    s.GetRequiredService<IHttpClientFactory>(),
                    s.GetRequiredService<IHttpContextAccessor>()));
                services.AddTransient<IRandom, RandomTrue>();
                services.AddTransient<IEmailSender, EmailSender>();
                services.AddSingleton<INow, Now>();
                services.AddSingleton<IStripeClient, StripeClient>(s => new StripeClient(Configuration.GetValue<string>("StripeSecretKey")));
            }
            else
            {
                services.AddSingleton<ICaptchaValidator, FakeCaptchaValidator>();
                services.AddSingleton<IRandom, RandomFake>();
                //We had it as singleton so we can get the content later during the asset phase
                services.AddSingleton<IEmailSender, FakeEmailSender>();
                services.AddSingleton<IStripeClient, FakeStripeClient>();
                services.AddSingleton<INow, FakeNow>();
            }
            services.AddAuthentication()
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["GoogleClientId"];
                    o.ClientSecret = Configuration["GoogleClientSecret"];
                });
            services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
                    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
                });
            services.AddMediatR(typeof(Startup), typeof(ChangePasswordCommand));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CaptchaMediatRAdapter<,>));
            services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

        }
        static Func<Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
            context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = (int)statusCode;
                    return Task.CompletedTask;
                }
                return existingRedirector(context);
            };
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var options = new RewriteOptions()
                .AddRewrite("^_framework/_bin/(.*)\\.blazor", "_framework/_bin/$1.dll", skipRemainingRules: true);

            app.UseRewriter(options);

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("fr"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            // will always save the current raven session after the mvc middleware
            app.Use(async (context, next) =>
             {
                 await next();
                 await app.ApplicationServices.GetService<IAsyncDocumentSession>().SaveChangesAsync();
             });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/api/{controller}/{action}/{id?}");
            });

            app.UseMiddleware<CsrfTokenCookieMiddleware>();

            app.UseBlazor<Toss.Client.Program>();
        }
    }
}