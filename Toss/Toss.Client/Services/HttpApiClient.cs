using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilder
    {
        private string _uri;
        private HttpClient _httpClient;
        private Func<HttpResponseMessage, Task> _onBadRequest;
        private Func<HttpResponseMessage, Task> _onOK;

        public HttpApiClientRequestBuilder(HttpClient httpClient, string uri)
        {
            _uri = uri;
            _httpClient = httpClient;

        }
        public async Task Post<T>(T data)
        {
            var requestJson = JsonUtil.Serialize(data);
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
            {
                Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
            });
            await HandleHttpResponse(response);

        }
        public async Task Post()
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
            {
            });
            await HandleHttpResponse(response);

        }
        private async Task HandleHttpResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    await _onOK(response);
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    await _onBadRequest(response);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    JsInterop.Redirect("/login");
                    break;
                    //other case , we do nothing, I'll add this case as needed
            }
        }

        public async Task Get()
        {

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, _uri));
            await HandleHttpResponse(response);
        }

        public HttpApiClientRequestBuilder OnBadRequest<T>(Action<T> todo)
        {
            _onBadRequest = async (HttpResponseMessage r) =>
            {
                var response = JsonUtil.Deserialize<T>(await r.Content.ReadAsStringAsync());
                todo(response);
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK<T>(Action<T> todo)
        {
            _onOK = async (HttpResponseMessage r) =>
            {
                var response = JsonUtil.Deserialize<T>(await r.Content.ReadAsStringAsync());
                todo(response);
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK(Action todo)
        {
            _onOK = async (HttpResponseMessage r) =>
            {
                todo();
            };
            return this;
        }

    }
}
