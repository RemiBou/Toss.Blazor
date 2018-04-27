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
        public static void Redirect(string url)
        {
            RegisteredFunction.Invoke<bool>("redirect", url);
        }
        public static void Toastr(string toastType,string message)
        {
            RegisteredFunction.Invoke<bool>("toastr",toastType, message);
        }
    }
}
