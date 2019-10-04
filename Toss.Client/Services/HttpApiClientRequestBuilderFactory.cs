using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilderFactory : IHttpApiClientRequestBuilderFactory
    {
        private readonly NavigationManager _navigationManager;
        private readonly HttpClient _httpClient;
        private readonly IBrowserCookieService browserCookieService;
        private readonly IJsInterop jsInterop;
        private readonly IMessageService messageService;

        public HttpApiClientRequestBuilderFactory(HttpClient httpClient, NavigationManager navigationManager, IBrowserCookieService browserCookieService, IJsInterop jsInterop, IMessageService messageService)
        {
            _navigationManager = navigationManager;
            _httpClient = httpClient;
            this.browserCookieService = browserCookieService;
            this.jsInterop = jsInterop;
            this.messageService = messageService;
        }
        public IHttpApiClientRequestBuilder Create(string url)
        {

            return new HttpApiClientRequestBuilder(_httpClient, url, _navigationManager, browserCookieService, jsInterop, messageService);
        }
    }
}
