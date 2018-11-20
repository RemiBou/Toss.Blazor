using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    /// <summary>
    /// Utility class for calling JS function declared in index.html
    /// </summary>
    public static class JsInterop
    {

        public static async Task<string> Captcha(string actionName)
        {
            return await JSRuntime.Current.InvokeAsync<string>("runCaptcha",actionName);
        }
        public static async Task Toastr(string toastType, string message)
        {
            await JSRuntime.Current.InvokeAsync<bool>("toastrShow", toastType, message);
        }

        public static async Task<string[]> Languages()
        {
            return await JSRuntime.Current.InvokeAsync<string[]>("navigatorLanguages");
        }
        public static async Task<int> AjaxLoaderShow(ElementRef elementRef)
        {
            return await JSRuntime.Current.InvokeAsync<int>("ajaxLoaderShow", elementRef);
        }
        public static async Task AjaxLoaderHide(int id)
        {
            await JSRuntime.Current.InvokeAsync<bool>("ajaxLoaderHide", id);
        }
        public static async Task ShowModal(ElementRef elementRef)
        {
            await JSRuntime.Current.InvokeAsync<bool>("showModal", elementRef);
        }
        public static async Task ShowModal(ElementRef elementRef, IModalCloseCallback closeCallback)
        {
            await JSRuntime.Current.InvokeAsync<bool>("showModal", elementRef, new DotNetObjectRef(closeCallback));
        }
        public static async Task HideModal(ElementRef elementRef)
        {
            await JSRuntime.Current.InvokeAsync<bool>("hideModal", elementRef);
        }
        private class StringHolder
        {
            public string Content { get; set; }
        }

        public static async Task<string> GetFileData(ElementRef fileInputRef)
        {
            return (await JSRuntime.Current.InvokeAsync<string>("getFileData", fileInputRef));
           
        }
        
        public static async Task<string> GetCookie()
        {
            return await JSRuntime.Current.InvokeAsync<string>("getDocumentCookie");
        }

        public static async Task OpenStripe(IStripeCallBack stripeCallBack, int amountInCts)
        {
            await JSRuntime.Current.InvokeAsync<string>("stripeCheckout",new DotNetObjectRef(stripeCallBack), amountInCts);
        }
    }
}
