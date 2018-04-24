using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationSample.Data;
using AuthenticationSample.Models;
using AuthenticationSample.Services;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Blazor.Server;

namespace AuthenticationSample
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
                    o.ClientId = "160749917187-us17f40a89uu77r1lhbqdvgb5idutfrh.apps.googleusercontent.com";
                    o.ClientSecret = "b6dySZ0MTJLEOrWyxNC-PkOC";
                });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

        }

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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseBlazor<Toss.Client.Program>();
        }
    }
}
