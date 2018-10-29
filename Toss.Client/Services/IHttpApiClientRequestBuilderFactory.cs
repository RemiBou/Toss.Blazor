using Microsoft.AspNetCore.Blazor;

namespace Toss.Client.Services
{
    public interface IHttpApiClientRequestBuilderFactory
    {
        IHttpApiClientRequestBuilder Create(string url, ElementRef elementRef = default(ElementRef));
    }
}