using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using Toss.Client.Services;
using System.Security.Claims;
using Microsoft.JSInterop;
using System.Globalization;
using System.Linq;

namespace Toss.Client
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");


            builder.Services.AddSingleton<IJsInterop, JsInterop>();
            builder.Services.AddScoped<IHttpApiClientRequestBuilderFactory, HttpApiClientRequestBuilderFactory>();
            builder.Services.AddSingleton<IBrowserCookieService, BrowserCookieService>();
            builder.Services.AddSingleton<II18nService, RemoteI18nService>();
            builder.Services.AddSingleton<IModelValidator, ModelValidator>();
            builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
            builder.Services.AddSingleton<IExceptionNotificationService, ExceptionNotificationService>();
            builder.Services.AddSingleton<IMessageService, MessageService>();
            builder.Services.AddSingleton<ApiAuthenticationStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>((s) => s.GetRequiredService<ApiAuthenticationStateProvider>());



            builder.Services.AddAuthorizationCore(options =>
                {
                    options.AddPolicy("HasLocalAccount", policy =>
                        policy.RequireAssertion(ctx => ctx.User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value == "internal"));
                });
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            var serviceCollection = builder.Build();
            var languages = await serviceCollection.Services.GetRequiredService<IJSRuntime>().InvokeAsync<string[]>("navigatorLanguages");
            CultureInfo.DefaultThreadCurrentCulture = languages.Select(c => CultureInfo.GetCultureInfo(c)).FirstOrDefault();
            await serviceCollection.RunAsync();
        }
    }
}
