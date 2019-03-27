using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Services;
using System.Net.Http;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilderFactory : IHttpApiClientRequestBuilderFactory
    {
        private readonly IUriHelper _uriHelper;
        private readonly HttpClient _httpClient;
        private readonly IBrowserCookieService browserCookieService;
        private readonly IJsInterop jsInterop;

        public HttpApiClientRequestBuilderFactory(HttpClient httpClient, IUriHelper uriHelper, IBrowserCookieService browserCookieService, IJsInterop jsInterop)
        {
            _uriHelper = uriHelper;
            _httpClient = httpClient;
            this.browserCookieService = browserCookieService;
            this.jsInterop = jsInterop;
        }
        public IHttpApiClientRequestBuilder Create(string url, ElementRef elementRef = default)
        {

            return new HttpApiClientRequestBuilder(_httpClient, url, _uriHelper, browserCookieService, jsInterop, elementRef);
        }
    }
}
