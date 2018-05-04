using Microsoft.AspNetCore.Blazor.Services;
using System.Net.Http;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilderFactory : IHttpApiClientRequestBuilderFactory
    {
        private readonly IUriHelper _uriHelper;
        private HttpClient _httpClient;
        public HttpApiClientRequestBuilderFactory(HttpClient httpClient, IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
            _httpClient = httpClient;

        }
        public IHttpApiClientRequestBuilder Create(string url)
        {
            return new HttpApiClientRequestBuilder(_httpClient, url, _uriHelper);
        }
    }
}
