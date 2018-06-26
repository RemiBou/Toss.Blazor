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
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Toss.Server.Models;
using Microsoft.AspNetCore.Identity.DocumentDB;

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
            services.AddSingleton(new DocumentClient(new Uri(Configuration["CosmosDBEndpoint"]), Configuration["CosmosDBKey"]));
            var client = services.BuildServiceProvider().GetRequiredService<DocumentClient>();
            Database db = client.CreateDatabaseQuery()
                                .Where(d => d.Id == Configuration["CosmosDBDataBaseid"])
                                .AsEnumerable()
                                .FirstOrDefault()
                ?? client.CreateDatabaseAsync(new Database { Id = Configuration["CosmosDBDataBaseid"] }).Result;

            services.AddIdentityWithDocumentDBStores<ApplicationUser,IdentityRole>(client,  db.SelfLink);



            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                            .ActionContext;
                return new UrlHelper(actionContext);
            });
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
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

          
            services.AddScoped<ITossRepository, TossCosmosRepository>();
            services.AddMediatR(typeof(Startup));
            services.AddMediatR(typeof(ChangePasswordCommand));


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
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/api/{controller}/{action}/{id?}");
            });

            app.UseBlazor<Toss.Client.Program>();
        }
    }
}
