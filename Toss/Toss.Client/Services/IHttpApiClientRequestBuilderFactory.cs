namespace Toss.Client.Services
{
    public interface IHttpApiClientRequestBuilderFactory
    {
        IHttpApiClientRequestBuilder Create(string url);
    }
}