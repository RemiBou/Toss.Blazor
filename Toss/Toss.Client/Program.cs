using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Globalization;
using System.Linq;
using Toss.Client.Services;

namespace Toss.Client
{
    public class Program
    {
        public static BrowserServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            serviceProvider = new BrowserServiceProvider(configure =>
            {
                configure.Add(new ServiceDescriptor(
                    typeof(IHttpApiClientRequestBuilderFactory),
                    typeof(HttpApiClientRequestBuilderFactory),
                    ServiceLifetime.Scoped));
                configure.Add(new ServiceDescriptor(
                    typeof(IAccountService),
                    typeof(AccountService),
                    ServiceLifetime.Scoped));
                configure.Add(new ServiceDescriptor(
                     typeof(IBrowserCookieService),
                     typeof(BrowserCookieService),
                     ServiceLifetime.Singleton));
                configure.Add(new ServiceDescriptor(
                    typeof(II18nService),
                    typeof(RemoteI18nService),
                    ServiceLifetime.Singleton));
                configure.Add(new ServiceDescriptor(
                    typeof(IModelValidator),
                    typeof(ModelValidator),
                    ServiceLifetime.Singleton));
            });


            JSRuntime.Current.InvokeAsync<string[]>("navigatorLanguages")
                .ContinueWith(t => CultureInfo.DefaultThreadCurrentCulture = t.Result.Select(c => CultureInfo.GetCultureInfo(c)).FirstOrDefault())
                .ContinueWith(t => new BrowserRenderer(serviceProvider).AddComponent<App>("app")) ;
        }
    }
}
