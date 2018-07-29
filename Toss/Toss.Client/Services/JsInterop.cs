using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    /// <summary>
    /// Utility class for calling JS function declared in index.html
    /// </summary>
    public static class JsInterop
    {
        public static async Task ConsoleLog(string message)
        {
            await JSRuntime.Current.InvokeAsync<bool>("console.log", message);
        }

        public static async Task Toastr(string toastType, string message)
        {
            await JSRuntime.Current.InvokeAsync<bool>("toastrShow", toastType, message);
        }
        public static async Task<int> AjaxLoaderShow(ElementRef elementRef)
        {
            return await JSRuntime.Current.InvokeAsync<int>("ajaxLoaderShow", elementRef);
        }
        public static async Task AjaxLoaderHide(int id)
        {
            await JSRuntime.Current.InvokeAsync<bool>("ajaxLoaderHide", id);
        }
        public static async Task ShowModal(string id)
        {
            await JSRuntime.Current.InvokeAsync<bool>("showModal", id);
        }
        private class StringHolder
        {
            public string Content { get; set; }
        }

        public static async Task<string> GetFileData(string fileInputRef)
        {
            return (await JSRuntime.Current.InvokeAsync<string>("getFileData", fileInputRef));
           
        }
        
        public static async Task<string> GetCookie()
        {
            return await JSRuntime.Current.InvokeAsync<string>("getDocumentCookie");
        }
    }
}
