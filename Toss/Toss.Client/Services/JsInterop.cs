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

        public static void Toastr(string toastType, string message)
        {
            RegisteredFunction.Invoke<bool>("toastr", toastType, message);
        }
        public static int AjaxLoaderShow(ElementRef elementRef)
        {
            return RegisteredFunction.Invoke<int>("ajaxLoaderShow", elementRef);
        }
        public static void AjaxLoaderHide(int id)
        {
            RegisteredFunction.Invoke<bool>("ajaxLoaderHide", id);
        }
        public static void ShowModal(string id)
        {
            RegisteredFunction.Invoke<bool>("showModal", id);
        }
        private class StringHolder
        {
            public string Content { get; set; }
        }

        public static async Task<string> GetFileData(string fileInputRef)
        {
            return (await RegisteredFunction.InvokeAsync<StringHolder>("getFileData", fileInputRef)).Content;
            //ConsoleLog("C# just received byte[] : " + base64fileContent.Length);
            //string res = null;
           

            //return res;
        }

    }
}
