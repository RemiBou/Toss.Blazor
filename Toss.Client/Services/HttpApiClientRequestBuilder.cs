using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Client.Services
{
    public class HttpApiClientRequestBuilder : IHttpApiClientRequestBuilder
    {
        private readonly string _relativeUri;
        private readonly string _uri;
        private readonly IUriHelper _uriHelper;
        private readonly HttpClient _httpClient;
        private Func<HttpResponseMessage, Task> _onBadRequest;
        private Func<HttpResponseMessage, Task> _onOK;
        private readonly IBrowserCookieService _browserCookieService;
        private readonly IMessageService _messageService;
        private readonly IJsInterop _jsInterop;

        public HttpApiClientRequestBuilder(HttpClient httpClient,
            string uri,
            IUriHelper uriHelper,
            IBrowserCookieService browserCookieService, IJsInterop jsInterop, IMessageService messageService)
        {
            _relativeUri = uri;
            _uri = uriHelper.ToAbsoluteUri(uri).ToString();
            _uriHelper = uriHelper;
            _httpClient = httpClient;

            _browserCookieService = browserCookieService;
            _jsInterop = jsInterop;
            _messageService = messageService;
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
            await SetCaptchaToken(data);
            await ExecuteHttpQuery(async () =>
            {
                string requestJson = JsonSerializer.ToString(data);
                return await _httpClient.SendAsync(await PrepareMessageAsync(new HttpRequestMessage(HttpMethod.Post, _uri)
                {
                    Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
                }));
            });
        }

        private async Task SetCaptchaToken<T>(T data)
        {
            if (data is NotARobot)
            {
                (data as NotARobot).Token = await _jsInterop.Captcha(_relativeUri);
            }
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
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    _messageService.Error("A server error occured, sorry");
                    break;
                    //other case , we do nothing, I'll add this case as needed
            }
        }
        private async Task<HttpRequestMessage> PrepareMessageAsync(HttpRequestMessage httpRequestMessage)
        {
            string csrfCookieValue = await _browserCookieService.Get(c => c.Equals("CSRF-TOKEN"));
            if (csrfCookieValue != null)
                httpRequestMessage.Headers.Add("X-CSRF-TOKEN", csrfCookieValue);
            httpRequestMessage.Headers.Add("X-Requested-With", "XMLHttpRequest");
            return httpRequestMessage;
        }
        public async Task Get()
        {
            await ExecuteHttpQuery(async () => await _httpClient.SendAsync(await PrepareMessageAsync(new HttpRequestMessage(HttpMethod.Get, _uri))));
        }
        private async Task ExecuteHttpQuery(Func<Task<HttpResponseMessage>> httpCall)
        {
            _messageService.Loading();
            HttpResponseMessage response = null;
            try
            {
                try
                {
                    response = await httpCall();
                }
                catch
                {
                    _messageService.Error("Connection error, server is down or you are not connected to the same network (internet).");
                    throw;
                }
                finally
                {
                    _messageService.LoadingDone();
                }
                await HandleHttpResponse(response);
            }
            finally
            {
                response?.Dispose();
            }
        }
        public HttpApiClientRequestBuilder OnBadRequest<T>(Action<T> todo)
        {
            _onBadRequest = async (HttpResponseMessage r) =>
            {
                T response = JsonSerializer.Parse<T>(await r.Content.ReadAsStringAsync());
                todo(response);
            };
            return this;
        }
        public HttpApiClientRequestBuilder OnOK<T>(Action<T> todo)
        {
            _onOK = async (HttpResponseMessage r) =>
            {
                T response = JsonSerializer.Parse<T>(await r.Content.ReadAsStringAsync());
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
                   _messageService.Info(successMessage);
               if (!string.IsNullOrEmpty(navigateTo))
                   _uriHelper.NavigateTo(navigateTo);
           });
            return this;
        }

        public void SetHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }
    }
}
