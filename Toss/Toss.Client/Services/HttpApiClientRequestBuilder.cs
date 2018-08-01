using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.JSInterop;
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
        private ElementRef _elementRef;
        private Func<HttpResponseMessage, Task> _onBadRequest;
        private Func<HttpResponseMessage, Task> _onOK;
        private IBrowserCookieService browserCookieService;

        public HttpApiClientRequestBuilder(HttpClient httpClient, string uri, IUriHelper uriHelper, IBrowserCookieService browserCookieService, ElementRef elementRef = default(ElementRef))
        {
            _uri = uri;
            this.uriHelper = uriHelper;
            _httpClient = httpClient;
            _elementRef = elementRef;
            this.browserCookieService = browserCookieService;
        }

        public async Task Post(byte[] data)
        {
            await ExecuteHttpQuery(async () =>
            {
                return await _httpClient.SendAsync(await PrepareMessageAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
                {
                    Content = new ByteArrayContent(data)
                }));
            });
        }
        public async Task Post<T>(T data)
        {
            await ExecuteHttpQuery(async () =>
            {
                var requestJson = Json.Serialize(data);
                return await _httpClient.SendAsync(await PrepareMessageAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
                {
                    Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
                }));
            });
        }
        public async Task Post()
        {
            await ExecuteHttpQuery(async () => await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _uri)));
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
                    await JsInterop.Toastr("error", "A server error occured, sorry");
                    break;
                    //other case , we do nothing, I'll add this case as needed
            }
        }
        private async Task<HttpRequestMessage> PrepareMessageAsync(HttpRequestMessage httpRequestMessage)
        {
            string csrfCookieValue = await browserCookieService.Get(c => c.Equals("CSRF-TOKEN"));
            if (csrfCookieValue != null)
                httpRequestMessage.Headers.Add("X-CSRF-TOKEN", csrfCookieValue);
            return httpRequestMessage;
        }
        public async Task Get()
        {
            await ExecuteHttpQuery(async () => await _httpClient.SendAsync(await PrepareMessageAsync(new HttpRequestMessage(HttpMethod.Get, _uri))));
        }
        private async Task ExecuteHttpQuery(Func<Task<HttpResponseMessage>> httpCall)
        {
            var loaderId = await JsInterop.AjaxLoaderShow(_elementRef);
            try
            {


                var response = await httpCall();
                await HandleHttpResponse(response);
            }
            catch
            {
                await JsInterop.Toastr("error", "Connection error, server is down or you are not connected to the same network.");
                throw;
            }
            finally
            {
                await JsInterop.AjaxLoaderHide(loaderId);
            }
        }
        public HttpApiClientRequestBuilder OnBadRequest<T>(Action<T> todo)
        {
            _onBadRequest = async (HttpResponseMessage r) =>
            {
                var response = Json.Deserialize<T>(await r.Content.ReadAsStringAsync());
                todo(response);
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK<T>(Action<T> todo)
        {
            _onOK = async (HttpResponseMessage r) =>
            {
                var response = Json.Deserialize<T>(await r.Content.ReadAsStringAsync());
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
            OnOK(async () =>
            {
                if (!string.IsNullOrEmpty(successMessage))
                    await JsInterop.Toastr("success", successMessage);
                uriHelper.NavigateTo(navigateTo);
            });
            return this;
        }

        public void SetHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }
    }
}
