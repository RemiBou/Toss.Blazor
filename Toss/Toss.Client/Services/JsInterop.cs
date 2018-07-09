using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    /// <summary>
    /// Utility class for calling JS function declared in index.html
    /// </summary>
    public static class JsInterop
    {
        public static void ConsoleLog(string message)
        {
            RegisteredFunction.Invoke<bool>("log", message);
        }
      
        public static void Toastr(string toastType,string message)
        {
            RegisteredFunction.Invoke<bool>("toastr",toastType, message);
        }
        public static int AjaxLoaderShow(ElementRef elementRef)
        {
            return RegisteredFunction.Invoke<int>("ajaxLoaderShow", elementRef);
        }
         public static void AjaxLoaderHide(int id)
        {
            RegisteredFunction.Invoke<bool>("ajaxLoaderHide",id);
        }
         public static void ShowModal(string id)
        {
            RegisteredFunction.Invoke<bool>("showModal",id);
        }

        public static async Task<string> Upload(byte[] fileContent)
        {
            string res = null;
            var httpRequestFactory = (IHttpApiClientRequestBuilderFactory)Program.serviceProvider.GetService(typeof(IHttpApiClientRequestBuilderFactory));
            await httpRequestFactory.Create("/api/picture/upload")
                .OnOK<string>(s => res = s)
                .Post(fileContent);
            return res;
        }
    }
}
