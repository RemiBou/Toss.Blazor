using Microsoft.AspNetCore.Components;

namespace Toss.Client.Services
{
    public interface IHttpApiClientRequestBuilderFactory
    {
        IHttpApiClientRequestBuilder Create(string url, ElementRef elementRef = default);
    }
}