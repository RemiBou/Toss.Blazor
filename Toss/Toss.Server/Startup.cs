using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationSample.Models;
using AuthenticationSample.Services;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Blazor.Server;
using AuthenticationSample.Data;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            // Add Elcamino Azure Table Identity services.

            services
                .AddIdentity<ApplicationUser, ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityRole>(
                    (options) =>
                    {
                        options.User.RequireUniqueEmail = true;
                        options.SignIn.RequireConfirmedEmail = true;
                    })
                .AddAzureTableStoresV2<ApplicationDbContext>(
                    () =>
                    {
                        IdentityConfiguration idconfig = new IdentityConfiguration();
                        idconfig.TablePrefix = "Auth";
                        idconfig.StorageConnectionString = "UseDevelopmentStorage=true;";
                        idconfig.LocationMode = "PrimaryOnly";
                        return idconfig;
                    })
                .AddDefaultTokenProviders()
                .CreateAzureTablesIfNotExists<ApplicationDbContext>(); //can remove after first run;

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
        }
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
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
