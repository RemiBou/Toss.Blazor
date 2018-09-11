using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Toss.Server.Services
{
    public interface IStripeClient
    {
        Task<bool> Charge(string token, int amount, string description, string email);
    }

    public class StripeClient : IStripeClient
    {
        private readonly HttpClient httpClient = new HttpClient();
        public async Task<bool> Charge(string token, int amount, string description, string email)
        {
            HttpRequestMessage query = new HttpRequestMessage(HttpMethod.Get, 
                $"https://api.stripe.com/v1/charges" +
                $"?amount={amount}" +
                $"&currency=eur" +
                $"&description={HttpUtility.UrlEncode(description)}" +
                $"&source={token}" +
                $"&receipt_email={email}");
            query.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("sk_test_x8Q4T7JGzApqkNORB7od8SHP:")));
            
            var response = await httpClient.SendAsync(query);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return json.GetValue("capture").Value<string>() == "true";

        }
    }
}
