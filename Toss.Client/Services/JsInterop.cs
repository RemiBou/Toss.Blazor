using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    /// <summary>
    /// Utility class for calling JS function declared in index.html
    /// </summary>
    public class JsInterop : IJsInterop
    {
        private readonly IJSRuntime jsRuntime;

        public JsInterop(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<string> Captcha(string actionName)
        {
            return await jsRuntime.InvokeAsync<string>("runCaptcha", actionName);
        }

        public async Task<string[]> Languages()
        {
            return await jsRuntime.InvokeAsync<string[]>("navigatorLanguages");
        }
       
        public async Task<string> GetCookie()
        {
            return await jsRuntime.InvokeAsync<string>("getDocumentCookie");
        }

        public async Task OpenStripe(IStripeCallBack stripeCallBack, int amountInCts)
        {
            await jsRuntime.InvokeAsync<string>("stripeCheckout", new DotNetObjectRef(stripeCallBack), amountInCts);
        }
    }
}
