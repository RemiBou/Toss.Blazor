using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Toss.Client.Services;

namespace Toss.Client
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IJsInterop, JsInterop>();
            services.Add(new ServiceDescriptor(
                    typeof(IHttpApiClientRequestBuilderFactory),
                    typeof(HttpApiClientRequestBuilderFactory),
                    ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(
                    typeof(IAccountService),
                    typeof(AccountService),
                    ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(
                     typeof(IBrowserCookieService),
                     typeof(BrowserCookieService),
                     ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(
                    typeof(II18nService),
                    typeof(RemoteI18nService),
                    ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(
                typeof(IModelValidator),
                typeof(ModelValidator),
                ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(
                typeof(IMarkdownService),
                typeof(MarkdownService),
                ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(
                typeof(IExceptionNotificationService),
                typeof(ExceptionNotificationService),
                ServiceLifetime.Singleton));
            services.AddSingleton<IMessageService, MessageService>();
            services.AddEnvironmentConfiguration<Startup>(() => 
                new EnvironmentChooser("Development")
                    .Add("localhost", "Development")
                    .Add("tossproject.com", "Production", false));
            
        }

        public void Configure(IComponentsApplicationBuilder app)
        {


            
            app.Services.GetRequiredService<IJSRuntime>().InvokeAsync<string[]>("navigatorLanguages")
                .ContinueWith(t => CultureInfo.DefaultThreadCurrentCulture = t.Result.Select(c => CultureInfo.GetCultureInfo(c)).FirstOrDefault())
                ;
            app.AddComponent<App>("app");
            app.InitEnvironmentConfiguration();
        }
    }
}
