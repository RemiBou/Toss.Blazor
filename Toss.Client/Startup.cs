using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
            services.AddScoped<IHttpApiClientRequestBuilderFactory, HttpApiClientRequestBuilderFactory>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IBrowserCookieService, BrowserCookieService>();
            services.AddSingleton<II18nService, RemoteI18nService>();
            services.AddSingleton<IModelValidator, ModelValidator>();
            services.AddSingleton<IMarkdownService, MarkdownService>();
            services.AddSingleton<IExceptionNotificationService,ExceptionNotificationService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddAuthorizationCore();
            services.AddSingleton<ApiAuthenticationStateProvider>();
            services.AddSingleton<AuthenticationStateProvider>((s) => s.GetRequiredService< ApiAuthenticationStateProvider>());
            
            services.AddEnvironmentConfiguration<Startup>(() => 
                new EnvironmentChooser("Development")
                    .Add("localhost", "Development")
                    .Add("tossproject.com", "Production", false));
            
        }

        public void Configure(IComponentsApplicationBuilder app)
        {   
            app.Services.GetRequiredService<IJSRuntime>().InvokeAsync<string[]>("navigatorLanguages")
                .AsTask().ContinueWith(t => CultureInfo.DefaultThreadCurrentCulture = t.Result.Select(c => CultureInfo.GetCultureInfo(c)).FirstOrDefault())
                ;
            app.AddComponent<App>("app");
            app.InitEnvironmentConfiguration();
        }
    }
}
