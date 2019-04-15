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
        private readonly IMessageService messageService;

        public HttpApiClientRequestBuilderFactory(HttpClient httpClient, IUriHelper uriHelper, IBrowserCookieService browserCookieService, IJsInterop jsInterop, IMessageService messageService)
        {
            _uriHelper = uriHelper;
            _httpClient = httpClient;
            this.browserCookieService = browserCookieService;
            this.jsInterop = jsInterop;
            this.messageService = messageService;
        }
        public IHttpApiClientRequestBuilder Create(string url)
        {

            return new HttpApiClientRequestBuilder(_httpClient, url, _uriHelper, browserCookieService, jsInterop, messageService);
        }
    }
}
