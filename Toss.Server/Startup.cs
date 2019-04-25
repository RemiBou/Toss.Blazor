using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Toss.Server.Data;
using MediatR;
using Toss.Server.Services;
using Toss.Shared.Account;
using Toss.Server.Models;
using Toss.Server.Extensions;
using System.Globalization;
using System.Net.Http;
using Raven.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using Raven.Client.Exceptions;

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
            AddRavenDBServices(services);
            // Add application services.
            if (Configuration.GetValue<string>("test") == null)
            {
                AddTrueDependencies(services);
            }
            else
            {
                AddFakeDependencies(services);
            }
            AddWebDependencies(services);
            AddMediatRDependencies(services);

        }

        private static void AddMediatRDependencies(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup), typeof(ChangePasswordCommand));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CaptchaMediatRAdapter<,>));
        }

        private void AddRavenDBServices(IServiceCollection services)
        {
            services.AddSingleton<IDocumentStore>(s =>
            {
                IDocumentStore store = new DocumentStore()
                {
                    Urls = new[] { Configuration.GetValue<string>("RavenDBEndpoint") },
                    Database = Configuration.GetValue<string>("RavenDBDataBase")
                };
                store.Conventions.FindCollectionName = type =>
                {
                    if (typeof(TossEntity).IsAssignableFrom(type))
                    {

                        return "TossEntity";
                    }
                    
                    return DocumentConventions.DefaultGetCollectionName(type);
                };
                
                store.Initialize();
                //taken from https://ravendb.net/docs/article-page/4.1/csharp/client-api/operations/server-wide/create-database
                try
                {
                    store.Maintenance.ForDatabase(store.Database).Send(new GetStatisticsOperation());
                }
                catch (DatabaseDoesNotExistException)
                {
                    try
                    {
                        store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
                    }
                    catch (ConcurrencyException)
                    {
                        // The database was already created before calling CreateDatabaseOperation
                    }
                }
                IndexCreation.CreateIndexes(typeof(Startup).Assembly, store);
                return store;
            });

            services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
            services.AddSingleton<RavenDBIdUtil>();
            services
                .AddRavenDbIdentity<ApplicationUser>();


        }

        private static void AddFakeDependencies(IServiceCollection services)
        {
            services.AddSingleton<ICaptchaValidator, FakeCaptchaValidator>();
            services.AddSingleton<IRandom, RandomFake>();
            //We had it as singleton so we can get the content later during the asset phase
            services.AddSingleton<IEmailSender, FakeEmailSender>();
            services.AddSingleton<IStripeClient, FakeStripeClient>();
            services.AddSingleton<INow, FakeNow>();
        }

        private void AddTrueDependencies(IServiceCollection services)
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

        private void AddWebDependencies(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddResponseCompression();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                            .ActionContext;
                return factory.GetService<IUrlHelperFactory>().GetUrlHelper(actionContext);
            });
            services.AddAuthentication()
                            .AddGoogle(o =>
                            {
                                o.ClientId = Configuration["GoogleClientId"];
                                o.ClientSecret = Configuration["GoogleClientSecret"];
                            });
            services.AddMvc(
                options =>
                {
                    options.Filters.Add<RavenDBSaveAsyncActionFilter>();
                }
                ).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                }).AddNewtonsoftJson();
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied = ReplaceRedirector(StatusCodes.Status403Forbidden, options.Events.OnRedirectToAccessDenied);
                options.Events.OnRedirectToLogin = ReplaceRedirector(StatusCodes.Status401Unauthorized, options.Events.OnRedirectToLogin);
            });
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
        }

        static Func<Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(int statusCode, Func<Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
            context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = statusCode;
                    return Task.CompletedTask;
                }
                return existingRedirector(context);
            };
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.EnvironmentName.Equals("Development"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseResponseCompression();

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
            app.UseAuthorization();


            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapDefaultControllerRoute();
            });

            app.UseMiddleware<CsrfTokenCookieMiddleware>();
            app.UseBlazor<Toss.Client.Program>();
        }
    }
}