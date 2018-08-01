using System;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public interface IHttpApiClientRequestBuilder
    {
        Task Get();
        HttpApiClientRequestBuilder OnBadRequest(Action todo);
        HttpApiClientRequestBuilder OnBadRequest(Func<Task> todo);
        HttpApiClientRequestBuilder OnBadRequest<T>(Action<T> todo);
        HttpApiClientRequestBuilder OnOK(Action todo);
        HttpApiClientRequestBuilder OnOK(Func<Task> todo);
        HttpApiClientRequestBuilder OnOK<T>(Action<T> todo);
        HttpApiClientRequestBuilder OnOK(string successMessage, string navigateTo = null);
        Task Post();
        Task Post<T>(T data);
        Task Post(byte[] data);
        void SetHeader(string v, string lg);
    }
}