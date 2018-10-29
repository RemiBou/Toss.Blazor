using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System.Net.Http;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilderFactory : IHttpApiClientRequestBuilderFactory
    {
        private readonly IUriHelper _uriHelper;
        private HttpClient _httpClient;
        private IBrowserCookieService browserCookieService;
        public HttpApiClientRequestBuilderFactory(HttpClient httpClient, IUriHelper uriHelper, IBrowserCookieService browserCookieService)
        {
            _uriHelper = uriHelper;
            _httpClient = httpClient;
            this.browserCookieService = browserCookieService;

        }
        public IHttpApiClientRequestBuilder Create(string url, ElementRef elementRef = default(ElementRef))
        {
            
            return new HttpApiClientRequestBuilder(_httpClient, url, _uriHelper, browserCookieService, elementRef);
        }
    }
}
