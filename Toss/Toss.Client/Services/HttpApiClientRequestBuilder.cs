using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilder : IHttpApiClientRequestBuilder
    {
        private string _uri;
        private readonly IUriHelper uriHelper;
        private HttpClient _httpClient;
        private Func<HttpResponseMessage, Task> _onBadRequest;
        private Func<HttpResponseMessage, Task> _onOK;

        public HttpApiClientRequestBuilder(HttpClient httpClient, string uri, IUriHelper uriHelper)
        {
            _uri = uri;
            this.uriHelper = uriHelper;
            _httpClient = httpClient;

        }
        public async Task Post<T>(T data)
        {
            var loaderId = JsInterop.AjaxLoaderShow();
            try
            {

                var requestJson = JsonUtil.Serialize(data);
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
                {
                    Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
                });
                await HandleHttpResponse(response);
            }
            finally
            {
                JsInterop.AjaxLoaderHide(loaderId);
            }

        }
        public async Task Post()
        {
            var loaderId = JsInterop.AjaxLoaderShow();
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
                {
                });
                await HandleHttpResponse(response);
            }
            finally
            {
                JsInterop.AjaxLoaderHide(loaderId);
            }

        }
        private async Task HandleHttpResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    if (_onOK != null)
                        await _onOK(response);
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    if (_onBadRequest != null)
                        await _onBadRequest(response);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    uriHelper.NavigateTo("/login");
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    JsInterop.Toastr("error","A server error occured, sorry");
                    break;
                    //other case , we do nothing, I'll add this case as needed
            }
        }

        public async Task Get()
        {

            var loaderId = JsInterop.AjaxLoaderShow();
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, _uri));
                await HandleHttpResponse(response);
            }
            finally
            {
                JsInterop.AjaxLoaderHide(loaderId);
            }
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
        public HttpApiClientRequestBuilder OnOK(Func<Task> todo)
        {
            _onOK = async (HttpResponseMessage r) =>
            {
                await todo();
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnBadRequest(Func<Task> todo)
        {
            _onBadRequest = async (HttpResponseMessage r) =>
            {
                await todo();
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK(Action todo)
        {
            _onOK = (HttpResponseMessage r) =>
            {
                todo();
                return Task.CompletedTask;
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnBadRequest(Action todo)
        {
            _onBadRequest = (HttpResponseMessage r) =>
            {
                todo();
                return Task.CompletedTask;
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK(string successMessage, string navigateTo = null)
        {
            OnOK(() =>
            {
                if (!string.IsNullOrEmpty(successMessage))
                    JsInterop.Toastr("success", successMessage);
                uriHelper.NavigateTo(navigateTo);
                return Task.CompletedTask;
            });
            return this;
        }

    }
}
