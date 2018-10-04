using Microsoft.AspNetCore.Blazor.Builder;
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


        }

        public void Configure(IBlazorApplicationBuilder app)
        {



            JSRuntime.Current.InvokeAsync<string[]>("navigatorLanguages")
                .ContinueWith(t => CultureInfo.DefaultThreadCurrentCulture = t.Result.Select(c => CultureInfo.GetCultureInfo(c)).FirstOrDefault())
                .ContinueWith(t => app.AddComponent<App>("app"));
        }
    }
}
